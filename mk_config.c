#include <stdio.h>
#include <utmp.h>
#include <stdlib.h> 

int main(int argc, char **argv)
{
   printf("namespace Linux.User {\n\tclass Config {\n");
   printf("\t\tpublic static string PathUtmp = \"%s\";\n",_PATH_UTMP);
   printf("\t\tpublic static string PathLastLog = \"%s\";\n",_PATH_LASTLOG);
   printf("\t\tpublic static int UtmpLength = %d;\n", sizeof(struct utmp));
   printf("\t\tpublic static int LastLogLength = %d;\n",sizeof(struct lastlog));
   printf("\t\tpublic static int UtLineSize = %d;\n",UT_LINESIZE);
   printf("\t\tpublic static int UtNameSize = %d;\n",UT_NAMESIZE);
   printf("\t\tpublic static int UtHostSize = %d;\n",UT_HOSTSIZE);
   printf("\t}\n");
   printf("}\n");
   exit(0);
}
