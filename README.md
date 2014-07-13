Linux.User
==========

Library to access linux user details with Mono c#

In order to build this you will need a working C compiler to build a small
prpgram that will create a small source file containing details of the
utmp and lastlog files.

You should be able to build this simply by typing

    make

by default it will use the v2.0 gmcs but you can change the Makefile to
change this.

If you want to install it in the global assemby cache you can type

   gacutil -i Linux.User.dll

probably with root permissions.

There is some example code in the examples directory, showing how some common
Unix utilities might be implemented using this library.
