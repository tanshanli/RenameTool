using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RenameTool
{
    class Program
    {
        static string srcProjectPath; // 原项目路径
        static string srcProjectName; // 原项目名称
        static string srcProjectNameRemarks; // 原项目名称备注（中文名）
        static string srcProjectNameFirstCharLowercase; // 原项目名称首字母小写
        static string srcProjectNameAllCharLowercase; // 原项目名称首字母全小写

        static string newProjectPath; // 新项目路径
        static string newProjectName; // 新项目名称
        static string newProjectNameRemarks; // 原项目名称备注（中文名）
        static string newProjectNameFirstCharLowercase; // 新项目名称首字母小写
        static string newProjectNameAllCharLowercase; // 新项目名称首字母全小写

        static Configuration config;
        //原项目名替换正则
        static Regex replaceRegexOriginalProjectName;
        //原项目备注（中文名）替换正则
        static Regex replaceRegexProjectNameRemarks;
        //原项目名首字母小写替换正则
        static Regex replaceRegexFirstCharLowercase;
        //原项目全小写替换正则
        static Regex replaceRegexAllCharLowercase;

        static void Main(string[] args)
        {

            Console.Title = "项目重命名工具";

            config = Configuration.Build();

            srcProjectPath = CmdReader.ReadLine("请输入原项目路径：", input => Directory.Exists(input));
            srcProjectName = CmdReader.ReadLine("请输入原项目名称：");
            srcProjectNameRemarks = CmdReader.ReadLine("请输入原项目中文名称：");
            srcProjectNameFirstCharLowercase = srcProjectName.FirstCharToLower();
            srcProjectNameAllCharLowercase = srcProjectName.ToLower();

            newProjectPath = CmdReader.ReadLine("请输入新项目保存位置：", input => Directory.Exists(input));
            newProjectName = CmdReader.ReadLine("请输入新项目名称：");
            newProjectNameRemarks = CmdReader.ReadLine("请输入新项目中文名：");
            newProjectNameFirstCharLowercase = newProjectName.FirstCharToLower();
            newProjectNameAllCharLowercase = newProjectName.ToLower();

            newProjectPath = Path.Combine(newProjectPath, newProjectName);

            Console.WriteLine("正在处理...");

            replaceRegexOriginalProjectName = new Regex(
                srcProjectName, RegexOptions.Multiline | RegexOptions.Compiled);
            replaceRegexProjectNameRemarks = new Regex(
               srcProjectNameRemarks, RegexOptions.Multiline | RegexOptions.Compiled);
            replaceRegexFirstCharLowercase = new Regex(
                srcProjectName.FirstCharToLower(), RegexOptions.Multiline | RegexOptions.Compiled);
            replaceRegexAllCharLowercase = new Regex(
                srcProjectName.ToLower(), RegexOptions.Multiline | RegexOptions.Compiled);

            // 保存到桌面
            // Environment.GetFolderPath(Environment.SpecialFolder.Desktop)

            Rename(srcProjectPath);

            Console.WriteLine($"完成！\r\n新的项目保存在：{newProjectPath}");
            Console.Write("请按任意键退出！");
            Console.ReadKey();
        }

        static void Rename(string srcDirectory, bool isCopyFolder = false)
        {
            string folderName = Path.GetFileName(srcDirectory);

            if (config.IgnoreFolders.Contains(folderName))
                return;

            isCopyFolder = isCopyFolder || config.CopyFolders.Contains(folderName);

            var directories = Directory.GetDirectories(srcDirectory);
            if (directories.Length > 0)
                Array.ForEach(directories, dir => Rename(dir, isCopyFolder));

            var files = Directory.GetFiles(srcDirectory);
            foreach (var file in files)
            {
                if (config.IgnoreExtensions.Contains(Path.GetExtension(file)))
                    continue;

                var destFile = file
                    .Replace(srcProjectPath, newProjectPath)
                    .Replace(srcProjectName, newProjectName);

                if (isCopyFolder)
                    CopyFile(file, destFile);
                else
                    ReplaceFile(file, destFile);
            }
        }

        static void CopyFile(string srcFile, string destFile)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destFile));
            File.Copy(srcFile, destFile, true);
        }

        static void ReplaceFile(string srcFile, string destFile)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destFile));

            //var encoding = Util.GetEncoding(srcFile);
            var encoding = Encoding.UTF8;
            var srcText = File.ReadAllText(srcFile, encoding);
            var destText = replaceRegexOriginalProjectName.Replace(srcText, newProjectName);
            destText = replaceRegexProjectNameRemarks.Replace(destText, newProjectNameRemarks);
            destText = replaceRegexFirstCharLowercase.Replace(destText, newProjectNameFirstCharLowercase);
            destText = replaceRegexAllCharLowercase.Replace(destText, newProjectNameAllCharLowercase);

            File.WriteAllText(destFile, destText, encoding);
        }
    }
}
