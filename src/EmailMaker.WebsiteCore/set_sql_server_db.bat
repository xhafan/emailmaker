@echo off
copy hibernate.cfg.xml.sqlserver hibernate.cfg.xml

rem The next command update the time stamp so visual studio can detect changed file - https://superuser.com/a/764721/68199
copy hibernate.cfg.xml+,,