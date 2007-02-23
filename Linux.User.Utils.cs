//
// Filename: Linux.User.Utils.cs
// 
// Module: Linux.User.dll
//
// Decription:
//    Various Utility Functions
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
// $Log: Linux.User.Utils.cs,v $
// Revision 1.2  2004/07/16 18:28:13  jonathan
// * Added header license to all file
// * Added Entries to the Linux.User.LastLog
//
// 
using System;
using System.Text;

namespace Linux.User
{
   ///<summary>
   /// Miscellaneous utility methods
   ///</summary>
   class Utils
   {
      /// <summary>
      /// Utility function to create a String from a C zero terminated
      /// string.
      /// </summary>
      public static string CCharsToString(char[] chars)
      {
         StringBuilder sb = new StringBuilder();

         foreach ( char c in chars)
         {
            if ((int)c == 0) break;
            sb.Append(c);
         }
         return sb.ToString();
      }
      ///<summary>
      /// Return a System.DateTime given a unix epoch seconds time_t
      ///</summary>
      public static DateTime EpochToDateTime(long e_secs)
      {
          DateTime d = new DateTime(1970,1,1);
          return d.AddSeconds(e_secs);
      }

   }
}
