//
// Filename:  Linux.User.Shadow.cs
// 
// Module: Linux.User.dll
//
// Decription:
//    Access to the shadow password file information
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
// $Log: Linux.User.Shadow.cs,v $
// Revision 1.2  2004/07/16 18:28:13  jonathan
// * Added header license to all file
// * Added Entries to the Linux.User.LastLog
//
// 
using System;
using System.IO;
using System.Text;

namespace Linux.User
{

   public class Shadow
   {
      private static string sp_file = "/etc/shadow";
      private FileStream fs;
      private StreamReader sr;

      public Shadow()
      {
         fs = new FileStream(sp_file,FileMode.Open,FileAccess.Read);
         sr = new StreamReader(fs, new UTF8Encoding());
      }

      ///<summary>
      /// Read entries from the shadow password file sequentially - will
      /// return null after the last entry is read.
      ///</summary>
      public ShadowEntry GetSPEnt()
      {
         string spent = sr.ReadLine();
         if ( spent == null )
         {
            return (ShadowEntry)null;
         }
         else
         {
            return new ShadowEntry(spent);
         }
      }
      ///<summary>
      /// Resets the password file stream back to the beginning
      ///</summary>
      public void SetSPEnt()
      {
         fs.Seek(0,SeekOrigin.Begin);
      }

      public ShadowEntry GetSPName(string Name)
      {
           long pos = fs.Position;
           this.SetSPEnt();
                                                                                                                    
           ShadowEntry spent = (ShadowEntry)null;
           while ((spent = this.GetSPEnt()) != null )
           {
               if (spent.Name == Name)
                  break;
           }

           fs.Seek(pos, SeekOrigin.Begin);
           return spent;
       }


   }

   public class ShadowEntry
   {
      ///<summary>
      /// Login name of user
      ///</summary>
      public string Name;

      ///<summary>
      /// Encrypted password - may be MD5 or DES
      ///</summary>
      public string Password;

      ///<summary>
      /// Date of last change in days since 01/01/1970
      ///</summary>
      public long LastChange;

      ///<summary>
      /// Minimum number of days allowed between changes
      ///</summary>
      public long Min;

      ///<summary>
      /// Maximum number of days allowed between changes
      ///</summary>
      public long Max;

      ///<summary>
      /// NUmber of days before the next change the user will be warned
      ///</summary>
      public long Warn;

      ///<summary>
      /// Number of days the account may be inactive before it is disabled
      ///</summary>
      public long Inactive;

      ///<summary>
      /// Days since last change that the password will
      /// expire.
      ///</summary>
      public long Expire;

      ///<summary>
      /// Reserved for future use
      ///</summary>
      public long Flag;

      public ShadowEntry()
      {
      }

      ///<summary>
      /// Takes a line from the shadow password file to initialize
      ///</summary>
      public ShadowEntry(string spent)
      {
         string[] entries = new string[9];
         entries = spent.Split(new char[]{':'}, 9 );
         
         this.Name = entries[0];
         this.Password = entries[1];
         try
         {
            this.LastChange = Convert.ToInt32(entries[2]);
            this.Min = Convert.ToInt32(entries[3]);
            this.Max = Convert.ToInt32(entries[4]);
            this.Warn = Convert.ToInt32(entries[5]);
            this.Inactive = Convert.ToInt32(entries[6]);
            this.Expire = Convert.ToInt32(entries[7]);
            this.Flag = Convert.ToInt32(entries[8]);
         }
         catch ( FormatException e )
         {
         }
      }
   }
}
