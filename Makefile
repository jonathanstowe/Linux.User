SRC = Linux.User.Utmp.cs \
		Linux.User.Password.cs \
		Linux.User.LastLog.cs \
		Linux.User.Shadow.cs \
		Linux.User.Utils.cs \
		AssemblyInfo.cs

CSC = mcs

Linux.User.dll:	$(SRC) Linux.User.Config.cs
	$(CSC) /t:library /out:Linux.User.dll $(SRC) Linux.User.Config.cs

Linux.User.Config.cs:	mk_config
	./mk_config > Linux.User.Config.cs

mk_config:	mk_config.c
	cc -o mk_config mk_config.c

clean:
	rm -f *.dll *~ mk_config Linux.User.Config.cs
