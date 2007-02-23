//
// Filename: Linux.User.Password.cs
// 
// Module: Linux.User.dll
//
// Decription:
//   Provide Access to user information
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
// $Log: Linux.User.Password.cs,v $
// Revision 1.2  2004/07/16 18:28:12  jonathan
// * Added header license to all file
// * Added Entries to the Linux.User.LastLog
//
// 
using System;
using System.IO;
using System.Text;

namespace Linux.User
{
   ///<summary>
   /// Deal with the password file
   ///</summary>
   public class Password
   {
      public static string pw_file = "/etc/passwd";

      private FileStream fs;
      private StreamReader sr;

      public Password()
      {
         fs = new FileStream(pw_file,FileMode.Open,FileAccess.Read);
         sr = new StreamReader(fs, new UTF8Encoding());
      }

      ///<summary>
      /// Reads the password entries sequentially - will return null
      /// after the last entry is read
      ///</summary>
      public PasswordEntry GetPWEnt()
      {
         string pwent = sr.ReadLine();
         if (pwent == null )
         {
            return (PasswordEntry)null;
         }
         else
         {
            return new PasswordEntry(pwent);
         }
      }

      ///<summary>
      /// Resets the password file stream back to the beginning
      ///</summary>
      public void SetPWEnt()
      {
         fs.Seek(0,SeekOrigin.Begin);
      }

      ///<summary>
      /// Return the PasswordEntry for the entry that matches the user id
      /// Will return null if no matching entry is found.
      ///</summary>
      public PasswordEntry GetPWUid(int Uid)
      {
         long pos = fs.Position;
         this.SetPWEnt();

         PasswordEntry pwent = (PasswordEntry)null;
         while ((pwent = this.GetPWEnt()) != null )
         {
            if (pwent.Uid == Uid)
               break;
         }

         fs.Seek(pos, SeekOrigin.Begin);
         return pwent;
      }
      ///<summary>
      /// Return the PasswordEntry for the entry that matches the user name
      /// Will return null if no matching entry is found.
      ///</summary>
      public PasswordEntry GetPWName(string Name)
      {
         long pos = fs.Position;
         this.SetPWEnt();

         PasswordEntry pwent = (PasswordEntry)null;
         while ((pwent = this.GetPWEnt()) != null )
         {
            if (pwent.Name == Name)
               break;
         }

         fs.Seek(pos, SeekOrigin.Begin);
         return pwent;
      }
   }

   ///<summary>
   /// Representation of entries in /etc/password
   ///</summary>
   public class PasswordEntry
   {
      ///<summary>
      /// User name
      ///</summary>
      public string Name;
      ///<summary>
      ///User password - will not be available if the user cannot read
      /// the secure password file
      ///</summary>
      public string Password;
      ///<summary>
      ///The numberic user id of the user
      ///</summary>
      public int Uid;
      ///<summary>
      ///The numeric primary group of the user
      ///</summary>
      public int Gid;
      ///<summary>
      ///Typically holds the description or full name of the user but
      /// may contain subfields on certain systems
      ///</summary>
      public string Gecos;
      ///<summary>
      ///The home directory of the user
      ///</summary>
      public string Dir;
      ///<summary>
      ///The shell program of the user
      ///</summary>
      public string Shell;

      ///<summary>
      ///Construct a Password from a raw row from the passwd file
      ///</summary>
      public PasswordEntry(string pwent)
      {
         string[] entries = new string[7];

         entries = pwent.Split(new char[]{':'}, 7);

         this.Name = entries[0];
         
         
         try
         {
            Shadow sp = new Shadow();
            ShadowEntry se;
            if ((se = sp.GetSPName(this.Name)) != null)
            {
               this.Password = se.Password;
            }
         }
         catch ( UnauthorizedAccessException e)
         {
            this.Password = entries[1]; /// This is obvious bollocks
         }
         this.Uid = Convert.ToInt32(entries[2]);
         this.Gid = Convert.ToInt32(entries[3]);
         this.Gecos = entries[4];
         this.Dir = entries[5];
         this.Shell = entries[6];
      }

      public PasswordEntry()
      {
      }
   }
}
