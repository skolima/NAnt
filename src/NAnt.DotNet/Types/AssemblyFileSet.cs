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
//
// Ian MacLean ( ian_maclean@another.com )

using System;
using System.Globalization;
using System.IO;
using System.Text;

using NAnt.Core.Attributes;
using NAnt.Core.Types;
using NAnt.Core.Util;

namespace NAnt.DotNet.Types {
    
    /// <summary>
    /// A specialized file set used for setting the Lib directories.
    /// </summary>
    /// <remarks>
    /// The primary reason for this class is to allow the BaseDirectory to always be the same value 
    /// as the parent AssemblyFileSet
    /// </remarks>
    [ElementName("assemblyfileset")]
    public class LibDirectorySet : FileSet {
        AssemblyFileSet _parent;
        #region Public Instance Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LibDirectorySet" /> class.
        /// </summary>
        /// <param name="parent"></param>
        public LibDirectorySet(AssemblyFileSet parent) {
            _parent = parent;
        }
        #endregion Public Instance Constructors
        
        #region Overrides from FileSet
        
        /// <summary>
        /// override this. We will always use the base directory of the parent.
        /// overriding without the TaskAttribute attribute prevents it being set 
        /// in the source xml
        /// </summary>
        public override DirectoryInfo BaseDirectory {
            get {return _parent.BaseDirectory; }
           
        }        
        #endregion Overrides from FileSet       
    }
    
    /// <summary>
    /// Specialized <see cref="FileSet" /> class for managing assembly files. 
    /// </summary>
    [ElementName("assemblyfileset")]
    public class AssemblyFileSet : FileSet, ICloneable {
        
        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyFileSet" /> class.
        /// </summary>
        public AssemblyFileSet() : base() {
            // set the parent reference to point back to us
            _lib = new LibDirectorySet(this);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyFileSet" /> class from
        /// the specified <see cref="FileSet" />.
        /// </summary>
        /// <param name="source">The <see cref="FileSet" /> that should be used to create a new instance of the <see cref="AssemblyFileSet" /> class.</param>
        public AssemblyFileSet(FileSet source) : base(source) {
        }

        #endregion Public Instance Constructors

        #region Public Instance Properties
                
        /// <summary>
        /// Additional directories to search in for assembly references.
        /// </summary>
        /// <remarks>
        /// <para>
        /// loosely Corresponds with the <c>/lib[path]:</c> flag of the various compiler tasks.
        /// </para>
        /// </remarks>
        [BuildElement("lib")]
        public LibDirectorySet Lib {
            get { return _lib; }
            set {_lib = value; }
        }
        
        #endregion Public Instance Properties
        
        #region Overrides from FileSet
        /// <summary>
        /// Do a normal scan and then resolve assemblies.
        /// </summary>
        public override void Scan() {
            base.Scan();
            
            ResolveReferences();            
        }
              
        #endregion Overrides from FileSet
        
        #region private intance methods
        
        /// <summary>
        /// Resolves references to system assemblies and assemblies that can be 
        /// resolved using directories specified in <see cref="Lib" />.
        /// </summary>       
        protected void ResolveReferences() {
            foreach (string pattern in Includes) {
                if (Path.GetFileName(pattern) == pattern) {
                    string localPath = Path.Combine(BaseDirectory.FullName, pattern);

                    // check if a file match the pattern exists in the 
                    // base directory of the references fileset
                    if (File.Exists(localPath)) {
                        // the file will already be included as part of
                        // the fileset scan process
                        continue;
                    }

                    foreach (string libPath in Lib.DirectoryNames) {
                        string fullPath = Path.Combine(libPath, pattern);

                        // check whether an assembly matching the pattern
                        // exists in the assembly directory of the current
                        // framework
                        if (File.Exists(fullPath)) {
                            // found a system reference
                            this.FileNames.Add(fullPath);

                            // continue with the next pattern
                            continue;
                        }
                    }

                    if (Project.TargetFramework != null) {
                        string frameworkDir = Project.TargetFramework.FrameworkAssemblyDirectory.FullName;
                        string fullPath = Path.Combine(frameworkDir, pattern);

                        // check whether an assembly matching the pattern
                        // exists in the assembly directory of the current
                        // framework
                        if (File.Exists(fullPath)) {
                            // found a system reference
                            this.FileNames.Add(fullPath);

                            // continue with the next pattern
                            continue;
                        }
                    }
                }
            }
        }
        #endregion private intance methods
        #region Private Instance Fields
        
        private LibDirectorySet _lib = null;

        #endregion Private Instance Fields

    }
}