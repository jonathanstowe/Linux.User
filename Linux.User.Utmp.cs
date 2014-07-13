//
// Filename: Linux.User.Utmp.cs
// 
// Module: Linux.User.dll
//
// Decription:
//    Access to the Utmp Login information
//
// Author:
//	Jonathan Stowe <jns@gellyfish.co.uk>
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
// $Log: Linux.User.Utmp.cs,v $
// Revision 1.2  2004/07/16 18:28:14  jonathan
// * Added header license to all file
// * Added Entries to the Linux.User.LastLog
//
// 
using System.IO;
using System;
using System.Text;
using System.Collections;

namespace Linux.User
{
   public class Utmp 
   {
      private FileStream sf;
      private BinaryReader br;

      private string ut_file = Config.PathUtmp;

      private int ut_len = Config.UtmpLength;

      private bool have_opened_files = false;

      public Utmp()
      {
      }

      public Utent[] Entries
      {
         get
         {
            ArrayList e = new ArrayList();

            while (this.MoreRecords())
            {
               e.Add(this.GetUtent());
            }

            Utent[] entries = new Utent[e.Count];
            e.CopyTo(entries);
            return entries;
         }
      }

      private void OpenFile()
      {
         sf = new FileStream(ut_file,FileMode.Open,FileAccess.Read);
         br = new BinaryReader(sf, new UTF8Encoding());
         this.have_opened_files = true;
      }

      ///<summary>
      /// Sets the name of the utmp file to be used - setting this will
      /// cause subsequent calls to  GetUtent to return entries from the
      /// new file.
      ///</summary>
      public void UtmpName(string ut_file)
      {
         this.EndUtent();
         this.ut_file = ut_file;
      }
      ///<summary>
      /// Closes all the filehandles associated with this object.
      ///</summary>
      public void EndUtent()
      {
         this.have_opened_files = false;
         this.br.Close();
         this.sf.Close();
      }

      /// <summary>
      /// Returns the next Utent from the utmp file
      /// will throw an Exception if there is no more data
      /// </summary>
      public Utent GetUtent()
      {
         if (! this.have_opened_files )
         {
            this.OpenFile();
         }

         Utent utent = new Utent();
         utent.ut_type = (UtType)br.ReadInt16();
         // 4 Byte alignment;
         sf.Seek(2,SeekOrigin.Current);
         utent.ut_pid = br.ReadInt32();
         utent.ut_line = Utils.CCharsToString(br.ReadChars(32));
         utent.ut_id = Utils.CCharsToString(br.ReadChars(4));
         utent.ut_user = Utils.CCharsToString(br.ReadChars(32));
         utent.ut_host = Utils.CCharsToString(br.ReadChars(256));
   
         ExitStatus e = new ExitStatus();
         e.e_termination = br.ReadInt16();
         e.e_exit = br.ReadInt16();
         utent.ut_exit = e;

         utent.ut_session = br.ReadInt32();
      
         TimeVal t = new TimeVal();
         t.tv_sec = br.ReadInt32();
         t.tv_usec = br.ReadInt32();
         utent.ut_tv = t;

         this.NextRecord();
         return utent;
      }

      ///<summary>
      /// Scan forward from the current position in the utmp file for
      /// entries with ut_type of UserProcess or LoginProcess and
      /// return the first one where ut_line is the same as Ut->ut_line
      /// Will return null if no matching entry is found.
      ///</summary>
      public Utent GetUtLine(Utent Ut)
      {
         Utent utent = (Utent)null;

         while (this.MoreRecords())
         {
            utent = this.GetUtent();
            if ( utent.ut_type == UtType.UserProcess || 
                  utent.ut_type == UtType.LoginProcess)
            {
               if ( utent.ut_line == Ut.ut_line )
                  break;
            }
         }
         return utent;
      }

      public Utent GetUtLine(string line)
      {
         Utent utent = new Utent();
         utent.ut_line = line;
         return GetUtLine(utent);
      }

      ///<summary>
      /// Rewind the input stream to the beginning
      ///</summary>
      public void SetUtent()
      {
         sf.Seek(0,SeekOrigin.Begin);
      }

      /// <summary>
      /// returns a true value if there are more records to be had
      /// </summary>
      public bool MoreRecords()
      {
         return this.have_opened_files ? sf.Position < sf.Length : true;
      }

      /// <summary>
      /// Positions the stream at the beginning of the next record
      /// </summary>
      public void NextRecord()
      {
         long rec = (this.sf.Position/this.ut_len) + 1;
         this.sf.Seek( (rec * this.ut_len) 
               - this.sf.Position,SeekOrigin.Current);
      }

   }

   ///<summary>
   /// A class describing the utmp entry - this is essentially the same
   /// format as a struct utent in C
   ///</summary>
   [Serializable]
   public class Utent
   {
      ///<summary>
      /// The type of this entry - see the UtType enum for details
      ///</summary>
      public UtType ut_type; 
      ///<summary>
      /// The process ID of the process for which this entry was created
      ///</summary>
      public long ut_pid;
      ///<summary>
      /// The device to which the process that the entry was created is
      /// connected - typically the login terminal
      ///</summary>
      public string ut_line;
      ///<summary>
      /// If the process for which this entry was created was spawned by
      /// the init process this will be the id from the inittab entry
      ///</summary>
      public string ut_id;
      ///<summary>
      /// The user name of the logged in user if this is a UserProcess
      /// Typically 'LOGIN' if this is a LoginProcess.
      ///</summary>
      public string ut_user;
      ///<summary>
      /// The hostname of the remote client for a remote login - will be
      /// $DISPLAY for an X session (e.g. xterm)
      ///</summary>
      public string ut_host;
      ///<summary>
      /// The exit status of the process for which the entry was created
      ///</summary>
      public ExitStatus ut_exit;
      ///<summary>
      /// A session Id typically used for windowing applications
      ///</summary>
      public int ut_session;
      ///<summary>
      ///The time at which this entry was created.
      ///</summary>
      public TimeVal ut_tv;
   }

   ///<summary>
   /// An equivalent of the C struct timeval
   ///</summary>
   public class TimeVal
   {
      ///<summary>
      /// Seconds since 1/1/1970 00:00:00
      ///</summary>
      public long tv_sec;
      ///<summary>
      /// Microseconds within the current second
      ///</summary>
      public long tv_usec;

      ///<summary>
      /// Returns a System.DateTime with second accuracy
      ///</summary>
      public DateTime Date
      {
         get
         {
            return Utils.EpochToDateTime(this.tv_sec);
         }
      }
   }

   /// <summary>
   /// Structure describing the status of a termnated process.
   /// Is 'exit_status' in the C structure
   /// </summary>
   //
   public struct ExitStatus
   {
      /// <summary>
      /// Process termination status
      /// </summary>
      public int e_termination;
      /// <summary>
      /// Process exit status
      /// </summary>
      public int e_exit;
   }

   /// <summary>
   /// Values for the ut_type field of Utent
   /// </summary>
   public enum UtType : int
   {
      /// <summary>
      /// No valid user accouting information
      /// </summary>
      Empty = 0,
      /// <summary>
      /// The Systems runlevel
      /// </summary>
      RunLevel = 1,
      /// <summary>
      /// Time of system boot
      /// </summary>
      BootTime = 2,
      /// <summary>
      /// Time after system clock changed
      /// </summary>
      NewTime = 3,
      /// <summary>
      /// Time when system clock changed
      /// </summary>
      OldTime = 4,
      /// <summary>
      /// Process spawned by init process
      /// </summary>
      InitProcess = 5,
      /// <summary>
      /// Session leader of a logged in user
      /// </summary>
      LoginProcess = 6,
      /// <summary>
      /// Normal Process
      /// </summary>
      UserProcess = 7,
      /// <summary>
      /// Terminated process
      /// </summary>
      DeadProcess = 8
   }
}
