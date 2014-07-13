SRC = Linux.User.Utmp.cs \
		Linux.User.Password.cs \
		Linux.User.LastLog.cs \
		Linux.User.Shadow.cs \
		Linux.User.Utils.cs \
		AssemblyInfo.cs

CSC = gmcs

Linux.User.dll:	$(SRC) Linux.User.Config.cs
	$(CSC) /t:library /out:Linux.User.dll -nowarn:0219 -keyfile:Linux.User.snk $(SRC) Linux.User.Config.cs

Linux.User.Config.cs:	mk_config
	./mk_config > Linux.User.Config.cs

mk_config:	mk_config.c
	cc -o mk_config mk_config.c

examples:	lastlog.exe

lastlog.exe:	Linux.User.dll
	$(CSC) -lib:. -r:Linux.User.dll -out:lastlog.exe examples/lastlog.cs
	
clean:
	rm -f *.exe *.dll *~ mk_config Linux.User.Config.cs
