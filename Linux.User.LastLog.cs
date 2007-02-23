//
// Filename: Linux.User.LastLog.cs
// 
// Module: Linux.User.dll
//
// Decription:
//    Access to user lastlog information
//
// Author:
//	Jonathan Stowe <jns@gellyfish.com>
//
// (c) 2004 Jonathan Stowe
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// $Log: Linux.User.LastLog.cs,v $
// Revision 1.2  2004/07/16 18:28:10  jonathan
// * Added header license to all file
// * Added Entries to the Linux.User.LastLog
//
// 
using System;
using System.IO;
using System.Text;
using System.Collections;

namespace Linux.User
{
   ///<summary>
   /// Access to the last login information per user
   /// </summary>
   [Serializable]
   public class LastLog
   {
      private FileStream fs;
      private BinaryReader br;

      private string ll_file = Config.PathLastLog;
      private int ll_len = Config.LastLogLength;

      private bool have_opened_files = false;

      public LastLogEntry[] Entries
      {
         get
         {
            ArrayList entries = new ArrayList();

            Password pw = new Password();

            PasswordEntry pwent;
            while ((pwent = pw.GetPWEnt()) != null )
            {
                  LastLogEntry llent = this.GetLlName(pwent.Name);
                  entries.Add(llent);
            }

            LastLogEntry[] llentries = new LastLogEntry[entries.Count];

            entries.CopyTo(llentries);
            return llentries;
         }

      }
      public LastLog()
      {
      }

      private void OpenFile()
      {
         fs = new FileStream(ll_file,FileMode.Open,FileAccess.Read);
         br = new BinaryReader(fs, new UTF8Encoding());
         this.have_opened_files = true;
      }

      ///<summary>
      /// Closes all the filehandles  associated with this object.
      ///</summary>
      public void EndLlEnt()
      { 
         if ( this.have_opened_files )
         {
            this.have_opened_files = false;
            this.br.Close();
            this.fs.Close();
         }
      }
                                    
      /// <summary>
      /// Set the lastlog stream to the beginning for rereading - opening the
      /// file if it is not already opened.
      /// </summary>
      public void SetLlEnt()
      {
         if ( !this.have_opened_files )
         {
            this.OpenFile();
         }

         this.fs.Seek(0,SeekOrigin.Begin );
      }

      /// <summary>
      /// Returns the next entry from the lastlog file
      /// it will return null if there is no more data.
      /// because the lastlog file is indexed on user id there will
      /// be discontinuities where there may be entries in the file
      /// for which there is no valid user id
      /// </summary>
      public LastLogEntry GetLlEnt()
      {
         if (! this.have_opened_files)
         {
            this.OpenFile();
         }

         LastLogEntry llent;

         if (this.fs.Position < this.fs.Length )
         {
            llent = GetCurrentEntry();
         }
         else
         {
            llent = (LastLogEntry)null;
         }

         return llent;

      }

      /// <summary>
      /// Returns and entry for the user indicated by user id or null if it
      /// is a uid higher than the current highest in the system.  That an
      /// entry is returned is not an indication that it is a valid UID as
      /// there may be a holes in the lastlog file for unused IDs
      /// </summary>
      public LastLogEntry GetLlUid(long uid)
      {
         LastLogEntry llent;

         if ( ! this.have_opened_files )
         {
            this.OpenFile();
         }

         long where = fs.Position;

         long uid_rec = uid * ll_len;

         if ( uid_rec >= this.fs.Length )
         {
            llent = (LastLogEntry)null;
         }
         else
         {
            this.fs.Seek(uid_rec,SeekOrigin.Begin);
            llent = GetCurrentEntry();
         }

         this.fs.Seek(where, SeekOrigin.Begin);
         return llent;
      }

      ///<summary>
      /// Returns the entry for the supplied username or null if an
      /// invalid username is given.
      ///</summary>

      public LastLogEntry GetLlName(string name)
      {
         Password pw = new Password();
         LastLogEntry llent = (LastLogEntry)null;
         PasswordEntry pwent = pw.GetPWName(name);

         if ( pwent != null )
         {
            llent = this.GetLlUid(pwent.Uid);
         }

         return llent;
      }


      private LastLogEntry GetCurrentEntry()
      {
         LastLogEntry llent = new LastLogEntry();
         llent.Uid = ( this.fs.Position/this.ll_len );
         llent.ll_time = this.br.ReadInt32();
         llent.Line = Utils.CCharsToString(br.ReadChars(Config.UtLineSize));
         llent.Host = Utils.CCharsToString(br.ReadChars(Config.UtHostSize));

         return llent;
      }

   }

      

   /// <summary>
   /// Describe a single entry from the lastlog file
   /// </summary>
   [Serializable]
   public class LastLogEntry
   {
      /// <summary>
      /// The user ID for whom this entry was made
      /// </summary>
      public long Uid;

      public long ll_time;
      /// <summary>
      /// The time of the users last login
      /// </summary>
      public DateTime Time
      {
         get
         {
            return Utils.EpochToDateTime(ll_time);
         }
      }

      /// <summary>
      /// The devicename of the terminal from which the login was made
      /// </summary>
      public string Line;

      /// <summary>
      /// The hostname from which the login was made - this will be blank
      /// if it was a local login or the value oF $DISPLAY if via an X
      /// session.
      /// </summary>
      public string Host;
   }
}
