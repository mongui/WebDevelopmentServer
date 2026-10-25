using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace WDS
{
    public enum Product { Unknown, Apache, nginx, lighttpd, NodeJS, IIS, PHP, MySQL, MariaDB }

    public enum ComponentKind { WebServer, PHP, DataBase }

    /// <summary>
    /// How PHP is connected to the web server.
    /// </summary>
    public enum PhpLink
    {
        None,
        ModPHP,     // Apache loads php*apache*.dll.
        ProxyFcgi,  // Apache 2.4.10+ sends .php to php-cgi.exe -b (mod_proxy_fcgi).
        Fcgid,      // Apache starts php-cgi.exe itself (mod_fcgid).
        Cgi,        // The web server runs php-cgi.exe for every request.
        FastCgi     // nginx/lighttpd send .php to php-cgi.exe -b.
    }

    public enum Severity { Info, Warning, Error }

    /// <summary>
    /// Password format of the database root user, chosen so the selected PHP can log in.
    /// </summary>
    public enum DbAuthMode { Default, Native, Old }

    public class Issue
    {
        public Severity Level;
        public String Text;

        public Issue(Severity level, String text)
        {
            this.Level = level;
            this.Text = text;
        }
    }

    /// <summary>
    /// A server version, either installed (a folder in bin\) or a package that can be downloaded.
    /// </summary>
    public class SoftwareComponent
    {
        public Product Product = Product.Unknown;
        public ComponentKind Kind;
        public String Name = "";
        public String Folder;           // null when it is not installed.
        public String Exe;
        public int[] Ver = { -1, -1, -1 };
        public int Bits;                // 32, 64 or 0 (unknown).
        public int VC;                  // Visual C++ version it was built with, 0 if unknown.

        // PHP only.
        public bool ThreadSafe;
        public bool HasCgiBinary;
        public String ExtensionDir;
        public List<String> Extensions;                      // null = unknown.
        public Dictionary<String, String> ApacheSapi = new Dictionary<String, String>(); // "2", "2_2", "2_4" -> dll ("" if not installed).

        // Apache only. null = unknown (not installed): the standard modules are assumed.
        public HashSet<String> Modules;

        public bool Installed { get { return this.Folder != null; } }

        public bool VersionKnown { get { return this.Ver[0] >= 0; } }

        public int V(int i)
        {
            return (i < 3 && this.Ver[i] >= 0) ? this.Ver[i] : 0;
        }

        public bool AtLeast(int major, int minor = 0, int patch = 0)
        {
            if (V(0) != major) return V(0) > major;
            if (V(1) != minor) return V(1) > minor;
            return V(2) >= patch;
        }

        public String VersionText
        {
            get
            {
                if (!this.VersionKnown) return "?";
                String t = V(0).ToString();
                if (this.Ver[1] >= 0) t += "." + this.Ver[1];
                if (this.Ver[2] >= 0) t += "." + this.Ver[2];
                return t;
            }
        }

        public String Title
        {
            get { return Compatibility.ProductName(this.Product) + " " + this.VersionText; }
        }

        public String Details
        {
            get
            {
                List<String> parts = new List<String>();
                if (this.Bits > 0) parts.Add(this.Bits + "-bit");
                if (this.VC > 0) parts.Add("VC" + this.VC);
                if (this.Product == Product.PHP) parts.Add(this.ThreadSafe ? "TS" : "NTS");
                return this.Title + (parts.Count > 0 ? " (" + String.Join(", ", parts.ToArray()) + ")" : "");
            }
        }
    }

    /// <summary>
    /// Result of joining two components.
    /// </summary>
    public class PairResult
    {
        public SoftwareComponent A;
        public SoftwareComponent B;
        public bool Compatible = true;

        // Web server + PHP.
        public PhpLink Link = PhpLink.None;
        public String SapiDll;

        // PHP + database.
        public DbAuthMode Auth = DbAuthMode.Default;
        public bool Utf8Fallback;

        public List<String> AutoConfig = new List<String>();  // What the program sets up.
        public List<String> Notes = new List<String>();       // Limits worth knowing.
        public List<String> Problems = new List<String>();    // Why they cannot work together.

        public void Fail(String why)
        {
            this.Compatible = false;
            this.Problems.Add(why);
        }

        public String Method
        {
            get
            {
                switch (this.Link)
                {
                    case PhpLink.ModPHP: return "PHP module (" + Path.GetFileName(this.SapiDll ?? "") + ")";
                    case PhpLink.ProxyFcgi: return "FastCGI (mod_proxy_fcgi)";
                    case PhpLink.Fcgid: return "FastCGI (mod_fcgid)";
                    case PhpLink.Cgi: return "CGI (slow, development only)";
                    case PhpLink.FastCgi: return "FastCGI";
                }
                return "";
            }
        }

        /// <summary>
        /// One line: how they are joined, or why they cannot be.
        /// </summary>
        public String Summary
        {
            get
            {
                if (!this.Compatible) return String.Join(" ", this.Problems.ToArray());
                List<String> all = new List<String>(this.AutoConfig);
                all.AddRange(this.Notes);
                return String.Join(" ", all.ToArray());
            }
        }
    }

    /// <summary>
    /// Compatibility of the selected (or installed) components with each other.
    /// </summary>
    public class Analysis
    {
        public SoftwareComponent Web;
        public SoftwareComponent Php;
        public SoftwareComponent Db;
        public PairResult WebPhp;
        public PairResult PhpDb;
        public Dictionary<SoftwareComponent, List<Issue>> Alone = new Dictionary<SoftwareComponent, List<Issue>>();

        public bool HasError(SoftwareComponent c)
        {
            if (c == null || !this.Alone.ContainsKey(c)) return false;
            foreach (Issue i in this.Alone[c])
            {
                if (i.Level == Severity.Error) return true;
            }
            return false;
        }

        public IEnumerable<String> Errors()
        {
            foreach (KeyValuePair<SoftwareComponent, List<Issue>> kv in this.Alone)
            {
                foreach (Issue i in kv.Value)
                {
                    if (i.Level == Severity.Error) yield return kv.Key.Title + ": " + i.Text;
                }
            }
            if (this.WebPhp != null && !this.WebPhp.Compatible)
                yield return this.WebPhp.A.Title + " + " + this.WebPhp.B.Title + ": " + this.WebPhp.Summary;
            if (this.PhpDb != null && !this.PhpDb.Compatible)
                yield return this.PhpDb.A.Title + " + " + this.PhpDb.B.Title + ": " + this.PhpDb.Summary;
        }

        public IEnumerable<String> Warnings()
        {
            foreach (KeyValuePair<SoftwareComponent, List<Issue>> kv in this.Alone)
            {
                foreach (Issue i in kv.Value)
                {
                    if (i.Level == Severity.Warning) yield return kv.Key.Title + ": " + i.Text;
                }
            }
        }

        public void Log()
        {
            foreach (KeyValuePair<SoftwareComponent, List<Issue>> kv in this.Alone)
            {
                foreach (Issue i in kv.Value)
                {
                    Globals.AddToLog((i.Level == Severity.Error ? "Error: " : i.Level == Severity.Warning ? "Warning: " : "") + kv.Key.Title + ": " + i.Text);
                }
            }
            foreach (PairResult p in new PairResult[] { this.WebPhp, this.PhpDb })
            {
                if (p == null) continue;
                String who = p.A.Title + " + " + p.B.Title;
                if (!p.Compatible)
                {
                    Globals.AddToLog("Incompatible: " + who + ". " + p.Summary);
                }
                else
                {
                    Globals.AddToLog(who + (p.Link != PhpLink.None ? " (" + p.Method + ")" : "") + ": " + String.Join(" ", p.AutoConfig.ToArray()));
                    foreach (String n in p.Notes) Globals.AddToLog("  " + n);
                }
            }
        }
    }

    public static class Compatibility
    {
        public const String FastCgiAddress = "127.0.0.1:8866";

        public static String ProductName(Product p)
        {
            switch (p)
            {
                case Product.Apache: return "Apache";
                case Product.nginx: return "nginx";
                case Product.lighttpd: return "lighttpd";
                case Product.NodeJS: return "Node.js";
                case Product.IIS: return "IIS Express";
                case Product.PHP: return "PHP";
                case Product.MySQL: return "MySQL";
                case Product.MariaDB: return "MariaDB";
            }
            return "Unknown";
        }

        public static ComponentKind KindOf(Product p)
        {
            switch (p)
            {
                case Product.PHP: return ComponentKind.PHP;
                case Product.MySQL:
                case Product.MariaDB: return ComponentKind.DataBase;
            }
            return ComponentKind.WebServer;
        }

        public static String BinFolder(ComponentKind kind)
        {
            switch (kind)
            {
                case ComponentKind.PHP: return Globals.AppFolder + @"bin\PHP\";
                case ComponentKind.DataBase: return Globals.AppFolder + @"bin\MySQL\";
            }
            return Globals.AppFolder + @"bin\WebServer\";
        }

        // ------------------------------------------------------------------
        // Building components.
        // ------------------------------------------------------------------

        /// <summary>
        /// The version selected in the main window, or null.
        /// </summary>
        public static SoftwareComponent Active(ComponentKind kind)
        {
            String name;
            switch (kind)
            {
                case ComponentKind.PHP: name = Globals.ActivePHPVersion; break;
                case ComponentKind.DataBase: name = Globals.ActiveMySQLVersion; break;
                default: name = Globals.ActiveWebServerVersion; break;
            }
            if (String.IsNullOrEmpty(name) || Globals.AppFolder == null) return null;

            String folder = BinFolder(kind) + name;
            return Directory.Exists(folder) ? FromFolder(kind, folder) : null;
        }

        /// <summary>
        /// Every installed version of one kind.
        /// </summary>
        public static List<SoftwareComponent> Installed(ComponentKind kind)
        {
            List<SoftwareComponent> list = new List<SoftwareComponent>();
            try
            {
                foreach (String dir in Directory.GetDirectories(BinFolder(kind)))
                {
                    list.Add(FromFolder(kind, dir));
                }
            }
            catch (Exception ex)
            {
                Globals.LogError("Unable to read the installed versions in " + BinFolder(kind), ex);
            }
            return list;
        }

        public static SoftwareComponent FromFolder(ComponentKind kind, String folder)
        {
            SoftwareComponent c = new SoftwareComponent();
            c.Kind = kind;
            c.Folder = folder.TrimEnd('\\');
            c.Name = Path.GetFileName(c.Folder);

            String[] files;
            switch (kind)
            {
                case ComponentKind.PHP: files = new PHPs().AvailableFiles; break;
                case ComponentKind.DataBase: files = new DataBases().AvailableFiles; break;
                default: files = new WebServers().AvailableFiles; break;
            }
            c.Exe = ServerType.FindServerFile(files, c.Folder + @"\");

            ReadName(c);
            if (c.Exe == null)
            {
                return c;
            }

            String exe = Path.GetFileName(c.Exe).ToLowerInvariant();
            switch (exe)
            {
                case "httpd.exe":
                case "apache.exe": c.Product = Product.Apache; break;
                case "nginx.exe": c.Product = Product.nginx; break;
                case "lighttpd.exe": c.Product = Product.lighttpd; break;
                case "node.exe": c.Product = Product.NodeJS; break;
                case "iisexpress.exe": c.Product = Product.IIS; break;
                case "php-cgi.exe":
                case "php.exe": c.Product = Product.PHP; break;
                default: c.Product = ServerType.IsMariaDB(c.Exe) ? Product.MariaDB : Product.MySQL; break;
            }

            // The executable knows its own version better than the folder name.
            try
            {
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(c.Exe);
                int[] v = ParseVersion(info.ProductVersion ?? info.FileVersion);
                if (v[0] > 0) c.Ver = v;
            }
            catch (Exception)
            {
            }

            try
            {
                bool? is64 = Is64Bit.GetBits.UnmanagedDllIs64Bit(c.Exe);
                if (is64.HasValue) c.Bits = is64.Value ? 64 : 32;
            }
            catch (Exception)
            {
            }

            String exeDir = Path.GetDirectoryName(c.Exe);
            if (c.Product == Product.PHP)
            {
                ReadPhpFolder(c, exeDir);
            }
            else if (c.Product == Product.Apache)
            {
                c.Modules = new HashSet<String>(StringComparer.OrdinalIgnoreCase);
                try
                {
                    String modules = Path.Combine(Directory.GetParent(exeDir).FullName, "modules");
                    if (Directory.Exists(modules))
                    {
                        foreach (String f in Directory.GetFiles(modules))
                        {
                            c.Modules.Add(Path.GetFileName(f));
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            DefaultVC(c);
            return c;
        }

        private static void ReadPhpFolder(SoftwareComponent c, String dir)
        {
            c.ThreadSafe = Directory.GetFiles(dir, "php*ts.dll").Length > 0;
            // PHP 4: php.exe in the root folder is the CGI binary (the CLI one is in cli\).
            c.HasCgiBinary = File.Exists(Path.Combine(dir, "php-cgi.exe")) ||
                             (c.V(0) == 4 && File.Exists(Path.Combine(dir, "php.exe")));

            foreach (String sub in new String[] { dir, Path.Combine(dir, "sapi") })
            {
                if (!Directory.Exists(sub)) continue;
                foreach (String dll in Directory.GetFiles(sub, "php*apache2*.dll"))
                {
                    String n = Path.GetFileNameWithoutExtension(dll).ToLowerInvariant();
                    String key = n.EndsWith("apache2_4") ? "2_4" : n.EndsWith("apache2_2") ? "2_2" : n.EndsWith("apache2") ? "2" : null;
                    if (key != null && !c.ApacheSapi.ContainsKey(key))
                    {
                        c.ApacheSapi.Add(key, dll);
                    }
                }
            }

            String ext = Path.Combine(dir, "ext");
            if (!Directory.Exists(ext) && Directory.Exists(Path.Combine(dir, "extensions")))
            {
                ext = Path.Combine(dir, "extensions"); // PHP 4.
            }
            c.ExtensionDir = ext;
            c.Extensions = new List<String>();
            if (Directory.Exists(ext))
            {
                foreach (String f in Directory.GetFiles(ext, "*.dll"))
                {
                    c.Extensions.Add(Path.GetFileName(f).ToLowerInvariant());
                }
            }
        }

        /// <summary>
        /// A package from the download list. Its name is the file name without extension,
        /// for example "php-8.4.0-nts-Win32-vs17-x64" or "httpd-2.2.25-win32-x86-no_ssl".
        /// </summary>
        public static SoftwareComponent FromPackage(Product product, String name)
        {
            SoftwareComponent c = new SoftwareComponent();
            c.Product = product;
            c.Kind = KindOf(product);
            c.Name = name ?? "";
            ReadName(c);

            if (product == Product.PHP)
            {
                // Official builds: TS packages have no "nts" in their name.
                c.ThreadSafe = !Regex.IsMatch(c.Name, @"(?i)(?<![a-z])nts(?![a-z])");
                c.HasCgiBinary = true;
                if (c.ThreadSafe)
                {
                    int major = c.V(0), minor = c.V(1);
                    if (major == 4 || (major == 5 && minor <= 4)) c.ApacheSapi["2"] = "";
                    if (major == 5 && minor >= 2 && minor <= 4) c.ApacheSapi["2_2"] = "";
                    if ((major == 5 && minor >= 5) || major >= 7) c.ApacheSapi["2_4"] = "";
                }
            }

            DefaultVC(c);
            return c;
        }

        public static Product ProductFromListName(String name)
        {
            String n = (name ?? "").ToLowerInvariant();
            if (n.Contains("php")) return Product.PHP;
            if (n.Contains("apache")) return Product.Apache;
            if (n.Contains("nginx")) return Product.nginx;
            if (n.Contains("lighttpd")) return Product.lighttpd;
            if (n.Contains("node")) return Product.NodeJS;
            if (n.Contains("iis")) return Product.IIS;
            if (n.Contains("maria")) return Product.MariaDB;
            if (n.Contains("mysql")) return Product.MySQL;
            return Product.Unknown;
        }

        /// <summary>
        /// Version, architecture and compiler from the folder or package name.
        /// </summary>
        private static void ReadName(SoftwareComponent c)
        {
            c.Ver = ParseVersion(c.Name);

            String n = c.Name.ToLowerInvariant();
            if (Regex.IsMatch(n, @"(x64|win64|winx64|amd64)")) c.Bits = 64;
            else if (Regex.IsMatch(n, @"(x86|win32|i386|i686)")) c.Bits = 32;

            Match vc = Regex.Match(n, @"(?<![a-z])(?:vc|vs)(\d{1,2})(?!\d)");
            if (vc.Success) c.VC = Int32.Parse(vc.Groups[1].Value);
        }

        /// <summary>
        /// Old official builds have no compiler in their name.
        /// </summary>
        private static void DefaultVC(SoftwareComponent c)
        {
            if (c.VC > 0 || !c.VersionKnown) return;
            if (c.Product == Product.Apache && !c.AtLeast(2, 4)) c.VC = 6;       // apache.org builds.
            if (c.Product == Product.PHP && !c.AtLeast(5, 3)) c.VC = 6;
            if (c.Product == Product.PHP && c.AtLeast(5, 3) && !c.AtLeast(5, 5)) c.VC = 9;
        }

        public static int[] ParseVersion(String text)
        {
            int[] v = { -1, -1, -1 };
            if (String.IsNullOrEmpty(text)) return v;
            Match m = Regex.Match(text, @"(\d+)\.(\d+)(?:\.(\d+))?");
            if (!m.Success) return v;
            v[0] = Int32.Parse(m.Groups[1].Value);
            v[1] = Int32.Parse(m.Groups[2].Value);
            if (m.Groups[3].Success) v[2] = Int32.Parse(m.Groups[3].Value);
            return v;
        }

        // ------------------------------------------------------------------
        // Rules.
        // ------------------------------------------------------------------

        public static Analysis Analyze(SoftwareComponent web, SoftwareComponent php, SoftwareComponent db)
        {
            Analysis a = new Analysis();
            a.Web = web;
            a.Php = php;
            a.Db = db;
            foreach (SoftwareComponent c in new SoftwareComponent[] { web, php, db })
            {
                if (c != null) a.Alone[c] = CheckAlone(c);
            }
            if (web != null && php != null) a.WebPhp = WebPhp(web, php);
            if (php != null && db != null) a.PhpDb = PhpDb(php, db);
            return a;
        }

        /// <summary>
        /// Problems of a component on this computer, whatever it is joined with.
        /// </summary>
        public static List<Issue> CheckAlone(SoftwareComponent c)
        {
            List<Issue> list = new List<Issue>();

            if (c.Product == Product.Unknown)
            {
                list.Add(new Issue(Severity.Error, "No server executable was found in this folder."));
                return list;
            }
            if (c.Product == Product.NodeJS || c.Product == Product.IIS)
            {
                list.Add(new Issue(Severity.Error, "This program cannot start " + ProductName(c.Product) + " yet."));
            }
            if (!c.VersionKnown)
            {
                list.Add(new Issue(Severity.Warning, "Unknown version: the compatibility checks may be wrong."));
            }
            if (c.Bits == 64 && !Environment.Is64BitOperatingSystem)
            {
                list.Add(new Issue(Severity.Error, "64-bit build: it cannot run on 32-bit Windows."));
            }

            String runtime = MissingRuntime(c.VC, c.Bits);
            if (runtime != null)
            {
                list.Add(new Issue(Severity.Error, runtime));
            }

            if (c.Installed && c.Exe != null)
            {
                String blocked = ControlledFolderWarning(c.Exe);
                if (blocked != null) list.Add(new Issue(Severity.Warning, blocked));
            }

            if (c.Product == Product.PHP && c.Installed && !c.HasCgiBinary)
            {
                list.Add(new Issue(Severity.Warning, "php-cgi.exe not found: PHP can only run as an Apache module."));
            }

            if (c.Kind == ComponentKind.DataBase && c.Installed && c.Exe != null && NeedsDataInit(c.Exe))
            {
                if (DataInitTool(c) != null)
                    list.Add(new Issue(Severity.Info, "The data folder will be created automatically on the first start."));
                else
                    list.Add(new Issue(Severity.Error, "The data folder is missing and this version has no tool to create it."));
            }

            return list;
        }

        /// <summary>
        /// Web server + PHP: picks the best working connection, or says why there is none.
        /// </summary>
        public static PairResult WebPhp(SoftwareComponent ws, SoftwareComponent php)
        {
            PairResult r = new PairResult();
            r.A = ws;
            r.B = php;

            if (ws.Product == Product.Unknown || php.Product != Product.PHP)
            {
                r.Fail("One of the components was not recognised.");
                return r;
            }

            String phpCgi = (php.V(0) == 4) ? "php.exe" : "php-cgi.exe";

            switch (ws.Product)
            {
                case Product.NodeJS:
                case Product.IIS:
                    r.Fail("This program does not connect PHP to " + ProductName(ws.Product) + ".");
                    return r;

                case Product.nginx:
                    if (php.V(0) < 5)
                    {
                        r.Fail("nginx needs PHP as a FastCGI server, and PHP 4 for Windows cannot run as one.");
                    }
                    else if (!php.HasCgiBinary)
                    {
                        r.Fail("php-cgi.exe was not found in the PHP folder.");
                    }
                    else
                    {
                        r.Link = PhpLink.FastCgi;
                        r.AutoConfig.Add("PHP runs as a FastCGI server (php-cgi.exe on " + FastCgiAddress + ") and nginx sends it the .php files.");
                    }
                    return r;

                case Product.lighttpd:
                    if (!php.HasCgiBinary)
                    {
                        r.Fail(phpCgi + " was not found in the PHP folder.");
                    }
                    else if (php.V(0) >= 5)
                    {
                        if (!HasModule(ws, "mod_fastcgi.dll", true))
                        {
                            r.Fail("lighttpd has no mod_fastcgi module.");
                        }
                        else
                        {
                            r.Link = PhpLink.FastCgi;
                            r.AutoConfig.Add("PHP runs as a FastCGI server (php-cgi.exe on " + FastCgiAddress + ") and lighttpd sends it the .php files.");
                        }
                    }
                    else if (!HasModule(ws, "mod_cgi.dll", true))
                    {
                        r.Fail("PHP 4 needs CGI and lighttpd has no mod_cgi module.");
                    }
                    else
                    {
                        r.Link = PhpLink.Cgi;
                        r.AutoConfig.Add("lighttpd runs php.exe for each request (CGI).");
                        r.Notes.Add("CGI is slow: use it only for development.");
                    }
                    return r;

                case Product.Apache:
                    break;

                default:
                    r.Fail(ProductName(ws.Product) + " is not supported.");
                    return r;
            }

            // Apache.
            String key = null;
            if (ws.V(0) == 2 && ws.V(1) == 0) key = "2";
            else if (ws.V(0) == 2 && ws.V(1) == 2) key = "2_2";
            else if (ws.V(0) == 2 && ws.V(1) == 4) key = "2_4";
            if (key == null)
            {
                r.Fail("Apache " + ws.VersionText + " is not supported (only 2.0, 2.2 and 2.4).");
                return r;
            }
            String apacheVer = ws.V(0) + "." + ws.V(1);

            // 1. PHP module: fastest, but it needs the same build options on both sides.
            String why = null;
            if (!php.ThreadSafe)
                why = "this PHP is a non thread safe (NTS) build and the module needs a thread safe (TS) one";
            else if (!php.ApacheSapi.ContainsKey(key))
                why = "PHP " + php.V(0) + "." + php.V(1) + " has no module for Apache " + apacheVer;
            else if (ws.Bits == 0 || php.Bits == 0)
                why = "the architecture of one of them is unknown";
            else if (ws.Bits != php.Bits)
                why = "Apache is " + ws.Bits + "-bit and PHP is " + php.Bits + "-bit";
            else if (!VcCompatible(ws.VC, php.VC))
                why = "they were built with different compilers (" + VcText(ws.VC) + " / " + VcText(php.VC) + ")";

            if (why == null)
            {
                r.Link = PhpLink.ModPHP;
                r.SapiDll = php.ApacheSapi[key];
                if (String.IsNullOrEmpty(r.SapiDll))
                {
                    r.SapiDll = "php" + php.V(0) + "apache" + key + ".dll";
                }
                r.AutoConfig.Add("Apache loads PHP as a module (" + Path.GetFileName(r.SapiDll) + ") with the php.ini of the PHP folder.");
                return r;
            }
            String notModule = "The PHP module is not used: " + why + ".";

            // 2. mod_proxy_fcgi (Apache 2.4.10+).
            if (php.V(0) >= 5 && php.HasCgiBinary && ws.AtLeast(2, 4, 10) &&
                HasModule(ws, "mod_proxy.so", true) && HasModule(ws, "mod_proxy_fcgi.so", true))
            {
                r.Link = PhpLink.ProxyFcgi;
                r.AutoConfig.Add("PHP runs as a FastCGI server (php-cgi.exe on " + FastCgiAddress + ") and Apache sends it the .php files (mod_proxy_fcgi).");
                r.Notes.Add(notModule);
                return r;
            }

            // 3. mod_fcgid, only if the user added it to Apache.
            if (php.V(0) >= 5 && php.HasCgiBinary && HasModule(ws, "mod_fcgid.so", false))
            {
                r.Link = PhpLink.Fcgid;
                r.AutoConfig.Add("Apache starts php-cgi.exe as FastCGI (mod_fcgid).");
                r.Notes.Add(notModule);
                return r;
            }

            // 4. CGI: always possible, but slow.
            if (php.HasCgiBinary && HasModule(ws, "mod_cgi.so", true) && HasModule(ws, "mod_actions.so", true))
            {
                r.Link = PhpLink.Cgi;
                r.AutoConfig.Add("Apache runs " + phpCgi + " for each request (mod_cgi + mod_actions).");
                r.Notes.Add(notModule);
                r.Notes.Add("CGI is slow: use it only for development.");
                return r;
            }

            r.Fail(notModule);
            if (!php.HasCgiBinary) r.Problems.Add(phpCgi + " was not found, so FastCGI and CGI are not possible either.");
            else r.Problems.Add("Apache has no mod_proxy_fcgi, mod_fcgid or mod_cgi + mod_actions to use instead.");
            return r;
        }

        /// <summary>
        /// PHP + database: PHP must be able to log in with the password format of the server.
        /// </summary>
        public static PairResult PhpDb(SoftwareComponent php, SoftwareComponent db)
        {
            PairResult r = new PairResult();
            r.A = php;
            r.B = db;

            if (php.Product != Product.PHP || db.Kind != ComponentKind.DataBase || db.Product == Product.Unknown)
            {
                r.Fail("One of the components was not recognised.");
                return r;
            }

            bool mysql = (db.Product == Product.MySQL);

            if (php.V(0) <= 4)
            {
                // PHP 4 only knows the pre-4.1 password format.
                if (mysql && db.AtLeast(5, 7))
                {
                    r.Fail("PHP 4 only knows the old password format, which MySQL removed in 5.7. Use MySQL 5.6 or older, or MariaDB.");
                    return r;
                }
                if (!mysql || db.AtLeast(4, 1))
                {
                    r.Auth = DbAuthMode.Old;
                    r.AutoConfig.Add("The root user gets an old-style password (old_passwords) so PHP 4 can log in.");
                    r.Notes.Add("Users you create yourself also need OLD_PASSWORD() to work with PHP 4.");
                }
            }
            else if (mysql && db.AtLeast(8) && !php.AtLeast(7, 4))
            {
                if (db.AtLeast(9))
                {
                    r.Fail("MySQL 9 removed mysql_native_password, and PHP " + php.V(0) + "." + php.V(1) +
                           " cannot use its replacement (caching_sha2_password needs PHP 7.4 or newer). Use MySQL 8, MariaDB or PHP 7.4+.");
                    return r;
                }
                r.Auth = DbAuthMode.Native;
                r.Utf8Fallback = true;
                r.AutoConfig.Add("The root user uses mysql_native_password (PHP older than 7.4 cannot use caching_sha2_password).");
                r.AutoConfig.Add("The server character set is utf8, because older PHP versions do not know utf8mb4 as default.");
                r.Notes.Add("Users you create yourself must use IDENTIFIED WITH mysql_native_password.");
            }

            if (php.Installed && php.V(0) >= 5 && php.Extensions != null)
            {
                bool any = php.Extensions.Contains("php_mysqli.dll") || php.Extensions.Contains("php_mysql.dll") ||
                           php.Extensions.Contains("php_pdo_mysql.dll");
                if (!any)
                {
                    r.Fail("The PHP folder has no MySQL extension (php_mysqli.dll, php_pdo_mysql.dll or php_mysql.dll).");
                    return r;
                }
            }

            r.AutoConfig.Add(php.V(0) <= 4
                ? "PHP 4 has MySQL support built in."
                : "The MySQL extensions of PHP are enabled automatically.");
            return r;
        }

        private static bool HasModule(SoftwareComponent ws, String module, bool assumeIfUnknown)
        {
            if (ws.Modules == null)
            {
                // Not installed: standard Apache packages have these modules.
                if (module.Equals("mod_proxy_fcgi.so", StringComparison.OrdinalIgnoreCase)) return ws.AtLeast(2, 4);
                return assumeIfUnknown;
            }
            if (ws.Product == Product.lighttpd && ws.Modules.Count == 0) return assumeIfUnknown;
            return ws.Modules.Contains(module);
        }

        private static bool VcCompatible(int a, int b)
        {
            if (a == 0 || b == 0) return false;
            if (a >= 14 && b >= 14) return true;    // VC14 and later share one runtime.
            return a == b;
        }

        private static String VcText(int vc)
        {
            return vc > 0 ? "VC" + vc : "unknown compiler";
        }

        // ------------------------------------------------------------------
        // Windows requirements.
        // ------------------------------------------------------------------

        /// <summary>
        /// Text of the missing Visual C++ runtime, or null if it is installed (or not needed).
        /// </summary>
        public static String MissingRuntime(int vc, int bits)
        {
            if (vc == 0 || bits == 0) return null;
            if (bits == 64 && !Environment.Is64BitOperatingSystem) return null; // Reported elsewhere.

            String arch = (bits == 64) ? "x64" : "x86";
            String dir = SystemDir(bits);

            if (vc >= 14)
            {
                String dll = Path.Combine(dir, "vcruntime140.dll");
                if (!File.Exists(dll))
                {
                    return "The Microsoft Visual C++ 2015-2022 Redistributable (" + arch + ") is not installed. Download it from https://aka.ms/vs/17/release/vc_redist." + arch + ".exe";
                }
                int needed = (vc >= 18) ? 40 : (vc >= 17) ? 30 : (vc >= 16) ? 20 : (vc >= 15) ? 10 : 0;
                try
                {
                    FileVersionInfo info = FileVersionInfo.GetVersionInfo(dll);
                    if (info.FileMajorPart == 14 && info.FileMinorPart < needed)
                    {
                        return "The Microsoft Visual C++ Redistributable (" + arch + ") is too old (" + info.FileMajorPart + "." + info.FileMinorPart +
                               "; 14." + needed + " or newer is needed). Update it from https://aka.ms/vs/17/release/vc_redist." + arch + ".exe";
                    }
                }
                catch (Exception)
                {
                }
                return null;
            }

            String old = null;
            switch (vc)
            {
                case 10: old = "msvcr100.dll|2010"; break;
                case 11: old = "msvcr110.dll|2012"; break;
                case 12: old = "msvcr120.dll|2013"; break;
            }
            if (old != null)
            {
                String[] p = old.Split('|');
                if (!File.Exists(Path.Combine(dir, p[0])))
                {
                    return "The Microsoft Visual C++ " + p[1] + " Redistributable (" + arch + ") is not installed.";
                }
            }
            return null;
        }

        /// <summary>
        /// Windows Security "Controlled folder access" stops unknown programs from writing in
        /// Documents, Desktop, Pictures... Servers there cannot write data, logs or PID files.
        /// Returns the warning text, or null.
        /// </summary>
        public static String ControlledFolderWarning(String exe)
        {
            if (String.IsNullOrEmpty(exe) || !ControlledFolderAccessOn()) return null;

            String full;
            try { full = Path.GetFullPath(exe); } catch (Exception) { return null; }

            foreach (Environment.SpecialFolder f in new Environment.SpecialFolder[] {
                Environment.SpecialFolder.MyDocuments, Environment.SpecialFolder.Desktop,
                Environment.SpecialFolder.MyPictures, Environment.SpecialFolder.MyVideos,
                Environment.SpecialFolder.MyMusic, Environment.SpecialFolder.Favorites })
            {
                String dir = Environment.GetFolderPath(f);
                if (!String.IsNullOrEmpty(dir) && full.StartsWith(dir.TrimEnd('\\') + "\\", StringComparison.OrdinalIgnoreCase))
                {
                    // Every program of the server that writes files must be allowed: MariaDB has
                    // mariadbd.exe and mysqld.exe, and the data folder is created by mysqld.exe.
                    List<String> names = new List<String>();
                    String bin = Path.GetDirectoryName(full);
                    foreach (String n in new String[] { Path.GetFileName(full), "mariadbd.exe", "mysqld.exe", "mariadb-install-db.exe", "mysql_install_db.exe" })
                    {
                        if (File.Exists(Path.Combine(bin, n)) && !names.Exists(x => x.Equals(n, StringComparison.OrdinalIgnoreCase))) names.Add(n);
                    }
                    return "Windows 'Controlled folder access' is on and this server is inside a protected folder (" + dir +
                           "), so Windows may stop it from writing its files. Move this program outside that folder, or in " +
                           "Windows Security > Ransomware protection > Allow an app, allow " + String.Join(", ", names.ToArray()) +
                           " from " + bin + ".";
                }
            }
            return null;
        }

        private static bool ControlledFolderAccessOn()
        {
            foreach (String key in new String[] {
                @"SOFTWARE\Policies\Microsoft\Windows Defender\Windows Defender Exploit Guard\Controlled Folder Access",
                @"SOFTWARE\Microsoft\Windows Defender\Windows Defender Exploit Guard\Controlled Folder Access" })
            {
                try
                {
                    using (Microsoft.Win32.RegistryKey baseKey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64))
                    using (Microsoft.Win32.RegistryKey k = baseKey.OpenSubKey(key))
                    {
                        if (k == null) continue;
                        object v = k.GetValue("EnableControlledFolderAccess");
                        if (v is int)
                        {
                            return (int)v == 1; // 2 = audit only.
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            return false;
        }

        private static String SystemDir(int bits)
        {
            String win = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            if (bits == 64)
            {
                return Path.Combine(win, Environment.Is64BitProcess ? "System32" : "Sysnative");
            }
            return Path.Combine(win, Environment.Is64BitOperatingSystem ? "SysWOW64" : "System32");
        }

        // ------------------------------------------------------------------
        // Database data folder.
        // ------------------------------------------------------------------

        public static String DataBaseRoot(String exe)
        {
            return Directory.GetParent(Path.GetDirectoryName(exe)).FullName;
        }

        /// <summary>
        /// MySQL 5.7+ and recent MariaDB packages come without the system tables.
        /// </summary>
        public static bool NeedsDataInit(String exe)
        {
            try
            {
                return !Directory.Exists(Path.Combine(DataBaseRoot(exe), @"data\mysql"));
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Program that creates the data folder, or null if there is none.
        /// </summary>
        public static String DataInitTool(SoftwareComponent db)
        {
            if (db.Exe == null) return null;
            if (db.Product == Product.MySQL)
            {
                return db.AtLeast(5, 7) ? db.Exe : null;
            }
            String bin = Path.GetDirectoryName(db.Exe);
            foreach (String tool in new String[] { "mariadb-install-db.exe", "mysql_install_db.exe" })
            {
                if (File.Exists(Path.Combine(bin, tool))) return Path.Combine(bin, tool);
            }
            return null;
        }

        // ------------------------------------------------------------------
        // Rules as text, for the user.
        // ------------------------------------------------------------------

        public static String[][] RulesTable()
        {
            return new String[][]
            {
                new String[] { "Apache 2.4 + PHP 5.5 or newer (TS, same bits)", "PHP module (php5/7/8apache2_4.dll)." },
                new String[] { "Apache 2.2 + PHP 5.2 to 5.4 (TS, same bits and compiler)", "PHP module (php5apache2_2.dll)." },
                new String[] { "Apache 2.0 + PHP 4 or 5.0 to 5.4 (TS, same bits and compiler)", "PHP module (php4apache2.dll / php5apache2.dll)." },
                new String[] { "Apache 2.4.10+ + PHP 5+ without a usable module (NTS, other bits...)", "FastCGI with mod_proxy_fcgi and php-cgi.exe." },
                new String[] { "Apache + mod_fcgid.so in its modules folder", "FastCGI with mod_fcgid, if there is no PHP module." },
                new String[] { "Any other Apache + PHP (for example Apache 2.2 + PHP 8)", "CGI with mod_cgi + mod_actions. Works, but slow." },
                new String[] { "nginx + PHP 5 or newer", "FastCGI with php-cgi.exe." },
                new String[] { "nginx + PHP 4", "Incompatible: PHP 4 for Windows cannot run as a FastCGI server." },
                new String[] { "lighttpd + PHP 5 or newer / PHP 4", "FastCGI / CGI." },
                new String[] { "Node.js or IIS Express + PHP", "Incompatible: not supported by this program." },
                new String[] { "PHP 7.4+ + any MySQL/MariaDB", "Works without changes." },
                new String[] { "PHP 5.x to 7.3 + MySQL 8.x", "root uses mysql_native_password and the utf8 character set." },
                new String[] { "PHP 5.x to 7.3 + MySQL 9 or newer", "Incompatible: MySQL 9 removed mysql_native_password." },
                new String[] { "PHP 4 + MySQL 4.1 to 5.6, or MariaDB", "root uses an old-style password (old_passwords)." },
                new String[] { "PHP 4 + MySQL 5.7 or newer", "Incompatible: the old password format was removed." },
                new String[] { "PHP 5 or newer without php_mysqli/php_pdo_mysql/php_mysql", "Incompatible with any database." },
                new String[] { "Web server + database", "Always compatible: they do not talk to each other." },
                new String[] { "64-bit builds", "Need 64-bit Windows." },
                new String[] { "VC14 to VS18 builds", "Need the Visual C++ 2015-2022 Redistributable of the same bits." },
                new String[] { "MySQL 5.7+ / MariaDB without a data folder", "The data folder is created on the first start." },
            };
        }
    }
}
