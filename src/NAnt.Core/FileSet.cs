// NAnt - A .NET build tool
// Copyright (C) 2001-2002 Gerry Shaw
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

// Gerry Shaw (gerry_shaw@yahoo.com)
// Ian MacLean (ian@maclean.ms)

using System;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Globalization;

using SourceForge.NAnt.Attributes;

namespace SourceForge.NAnt {

    /// <summary>
    /// The FileSet element.
    /// </summary>
    /// <remarks>
    /// Used as a child element in various file-related tasks, including delete, copy, touch, get, atrrib, move...
    /// </remarks>
    /// <history>
    /// <change date="20030224" author="Brian Deacon (bdeacon at vidya dot com">Added support for the failonempty attribute</change>
    /// </history>
    public class FileSet : Element {

        // copy constructor 
        public FileSet( FileSet source)        { 			
            _defaultExcludes = source._defaultExcludes;		
            _location = source._location;
            _parent = source._parent;			
            _project = source._project;			
            if ( _xmlNode != null )
                _xmlNode = source._xmlNode.Clone();
        }		
        
        /// <summary>Determines if a file has a more recent last write time than the given time.</summary>
        /// <param name="fileNames">A collection of filenames to check last write times against.</param>
        /// <param name="targetLastWriteTime">The datetime to compare against.</param>
        /// <returns>The name of the first file that has a last write time greater than <c>targetLastWriteTime</c>; otherwise null.</returns>
        public static string FindMoreRecentLastWriteTime(StringCollection fileNames, DateTime targetLastWriteTime) {
            foreach (string fileName in fileNames) {
                // only check fully file names that have a full path
                if (Path.IsPathRooted(fileName)) {
                    FileInfo fileInfo = new FileInfo(fileName);
                    if (!fileInfo.Exists) {
                        return fileName;
                    }
                    if (fileInfo.LastWriteTime > targetLastWriteTime) {
                        return fileName;
                    }
                }
            }
            return null;
        }

        bool _hasScanned = false;
        bool _defaultExcludes = true;
        bool _failOnEmpty = false;
        DirectoryScanner _scanner = new DirectoryScanner();
        StringCollection _asis = new StringCollection();
        PathScanner _pathFiles = new PathScanner();		

        public FileSet() {
        }

        /// <summary>
        /// When set to true, causes the fileset element to throw a ValidationException when no files match 
        /// the includes and excludes criteria. Default "false".
        /// </summary>
        [TaskAttribute("failonempty")]
        [BooleanValidator()]
        public bool FailOnEmpty {
            get { return _failOnEmpty; }
            set { _failOnEmpty = value; }
        }

        /// <summary>Indicates whether default excludes should be used or not.  Default "true".</summary>
        [TaskAttribute("defaultexcludes")]
        [BooleanValidator()]
        public bool DefaultExcludes {
            get { return _defaultExcludes; }
            set { _defaultExcludes = value; }
        }

        /// <summary>The base of the directory of this file set.  Default is project base directory.</summary>
        [TaskAttribute("basedir")]
        public string BaseDirectory {
            get { return _scanner.BaseDirectory; }
            set { _scanner.BaseDirectory = value; }
        }

        public StringCollection Includes {
            get { return _scanner.Includes; }
        }

        public StringCollection Excludes {
            get { return _scanner.Excludes; }
        }

        public StringCollection AsIs {
            get { return _asis; }
        }

        public PathScanner PathFiles {
            get { return _pathFiles; }
        }

        /// <summary>The collection of file names that match the file set.</summary>
        public StringCollection FileNames {
            get {
                if (!_hasScanned) {
                    Scan();
                }
                return _scanner.FileNames;
            }
        }

        public void Scan() {
            try {
                _scanner.Scan();

                // Add all the as-is patterns to the scanned files.
                foreach (string name in AsIs) {
                    _scanner.FileNames.Add(name);
                }

                // Add all the path-searched patterns to the scanned files.
                foreach (string name in PathFiles.Scan()) {
                    _scanner.FileNames.Add(name);
                }
            } catch (Exception e) {
                throw new BuildException("Error creating file set.", Location, e);
            }
            _hasScanned = true;

            if (FailOnEmpty && _scanner.FileNames.Count == 0) {
                throw new ValidationException("The fileset specified is empty.", Location);
            }
        }

        protected override void InitializeElement(XmlNode elementNode)  {
            if (BaseDirectory == null) {
                BaseDirectory = Project.BaseDirectory;
            } else {
                BaseDirectory = Project.GetFullPath(BaseDirectory);
            }

            if (DefaultExcludes) {
                // add default exclude patterns
                Excludes.Add("**/*~");
                Excludes.Add("**/#*#");
                Excludes.Add("**/.#*");
                Excludes.Add("**/%*%");
                Excludes.Add("**/CVS");
                Excludes.Add("**/CVS/**");
                Excludes.Add("**/.cvsignore");
                Excludes.Add("**/SCCS");
                Excludes.Add("**/SCCS/**");
                Excludes.Add("**/vssver.scc");
                Excludes.Add("**/_vti_cnf/**");
            }

            // The Element class will initialize the marked xml attributes but
            // not the unmarked <includes> and <excludes> elements.  We have to
            // initialize them ourselves.

            foreach (XmlNode node in elementNode) 
            {
                if(node.Name.Equals("includes"))
                {
                    IncludesElement include = new IncludesElement();
                    include.Project = Project;
                    include.Initialize(node);

                    if (include.IfDefined && !include.UnlessDefined) 
                    {
                        if (include.AsIs) 
                        {
                            AsIs.Add(include.Pattern);
                        } 
                        else if (include.FromPath) 
                        {
                            PathFiles.Add(include.Pattern);
                        } 
                        else 
                        {
                            Includes.Add(include.Pattern);
                        }
                    }
                }
                else if(node.Name.Equals("excludes"))
                {
                    ExcludesElement exclude = new ExcludesElement();
                    exclude.Project = Project;
                    exclude.Initialize(node);

                    if (exclude.IfDefined && !exclude.UnlessDefined) 
                    {
                        Excludes.Add(exclude.Pattern);
                    }
                }
                else if(node.Name.Equals("includesList"))
                {
                    IncludesListElement include = new IncludesListElement();
                    include.Project=Project;
                    include.Initialize(node);
                    if (include.IfDefined && !include.UnlessDefined)
                    {
                        foreach(string s in include.Files)
                            AsIs.Add(s);
                    }
                }
            }

        }

        // These classes provide a way of getting the Element task to initialize
        // the values from the build file.

        [ElementName("excludes")]
        class ExcludesElement : Element {
            string _pattern;
            bool _ifDefined = true;
            bool _unlessDefined = false;

            /// <summary>The pattern or file name to include.</summary>
            [TaskAttribute("name", Required=true)]
            public string Pattern {
                get { return _pattern; }
                set { _pattern= value; }
            }

            /// <summary>If true then the pattern will be included; otherwise skipped. Default is "true".</summary>
            [TaskAttribute("if")]
            [BooleanValidator()]
            public bool IfDefined {
                get { return _ifDefined; }
                set { _ifDefined = value; }
            }

            /// <summary>Opposite of if.  If false then the pattern will be included; otherwise skipped. Default is "false".</summary>
            [TaskAttribute("unless")]
            [BooleanValidator()]
            public bool UnlessDefined {
                get { return _unlessDefined; }
                set { _unlessDefined = value; }
            }
        }

        [ElementName("includes")]
        class IncludesElement : ExcludesElement {
            bool _asIs = false;
            bool _fromPath = false;

            /// <summary>If true then the file name will be added to the file set without pattern matching or checking if the file exists.</summary>
            [TaskAttribute("asis")]
            [BooleanValidator()]
            public bool AsIs {
                get { return _asIs; }
                set { _asIs = value; }
            }

            /// <summary>If true then the file will be searched for on the path.</summary>
            [TaskAttribute("frompath")]
            [BooleanValidator()]
            public bool FromPath {
                get { return _fromPath; }
                set { _fromPath = value; }
            }
        }
        
        [ElementName("fromfile")]
        class IncludesListElement : ExcludesElement {

            string[] files;
            public string[] Files {
                get { return files; }
            }

            new public void Initialize(XmlNode elementNode)
            {
                base.Initialize(elementNode);
                StringCollection f=new StringCollection();

                System.IO.Stream fil=File.OpenRead(Pattern);
                if(fil==null) {
                    throw new BuildException(String.Format(CultureInfo.InvariantCulture, "'{0}' list could not be opened",Pattern));
                }
                System.IO.StreamReader rd=new System.IO.StreamReader(fil);
                while(rd.Peek()>-1) {
                    f.Add(rd.ReadLine());
                }

                files=new string[f.Count];
                f.CopyTo(files,0);
            }
        }
    }
}
