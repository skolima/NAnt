// NAnt - A .NET build tool
// Copyright (C) 2003 Scott Hernandez
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Scott Hernandez (ScottHernandez@hotmail.com)

using System;
using System.IO;
using SourceForge.NAnt.Attributes;

using ICSharpCode.SharpZipLib.Zip;

namespace SourceForge.NAnt.Tasks {
    
    /// <summary>
    /// A task to extract file from a zip file.
    /// </summary>
    /// <remarks>
    ///   <para>Uses <a href="http://www.icsharpcode.net/OpenSource/SharpZipLib/">#ziplib</a> (SharpZipLib), an open source Zip/GZip library written entirely in C#.</para>
    /// </remarks>
    /// <example>
    ///   <para>Extracts all the file from the zip, preserving the directory structure.</para>
    ///   <code>
    ///     <![CDATA[
    /// <unzip zipfile="backup.zip"/>
    ///     ]]>
    ///   </code>
    /// </example>
    [TaskName("unzip")]
    public class UnZipTask : Task {
        #region Private Instance Fields

        string _zipfile = null;
        string _toDir = ".";

        #endregion Private Instance Fields

        #region Public Instance Properties

        /// <summary>The zip file to use.</summary>
        [TaskAttribute("zipfile", Required=true)]
        public string ZipFileName { get { return Project.GetFullPath(_zipfile); } set {_zipfile = value; } }

        /// <summary>The zip file to use.</summary>
        [TaskAttribute("todir", Required=false)]
        public string ToDir { get { return Project.GetFullPath(_toDir ); } set {_toDir = value; } }

        #endregion Public Instance Properties

        #region Override implementation of Task
        
        protected override void ExecuteTask() {
            ZipInputStream s = new ZipInputStream(File.OpenRead(ZipFileName));
            Log.WriteLine(LogPrefix + "Unzipping {0} to {1} ({2} bytes)", _zipfile, _toDir, s.Length);
            ZipEntry theEntry;
            while ((theEntry = s.GetNextEntry()) != null) {                string directoryName = Path.GetDirectoryName(theEntry.Name);                string fileName      = Path.GetFileName(theEntry.Name);
                Log.WriteLineIf(Verbose, "Extracting {0} to {1}", theEntry.Name, _toDir);
                // create directory                DirectoryInfo currDir = Directory.CreateDirectory(Path.Combine(ToDir, directoryName));
                if (fileName != null && fileName.Length != 0) {                    FileInfo fi = new FileInfo(Path.Combine(currDir.FullName, fileName));                    FileStream streamWriter = fi.Create();                    int size = 2048;                    byte[] data = new byte[2048];
                    while (true) {                        size = s.Read(data, 0, data.Length);                        if (size > 0) {                            streamWriter.Write(data, 0, size);                        } else {                            break;                        }                    }
                    streamWriter.Close();                    fi.LastWriteTime = theEntry.DateTime;                }            }            s.Close();
        }

        #endregion Override implementation of Task
    }
}
