# Pick'em
Pick'em is a questionable system that allows the picking of NCAAF games based on the spread.

### Development Environment Setup
Here is some incomplete documentation on how to setup a development environment. Feel free to contact me if you have questions.

Pick'em is comprised of:
- a [postgresql](https://www.postgresql.org/) database
- a server built on [.NET Core](https://docs.microsoft.com/en-us/dotnet/core/)
- a web site built with [Angular](https://angular.io/)
- a CLI built with [python](https://www.python.org/)
- There is also the start of an Android app

#### Development Tools
I do development with the following
- Windows 10
- VS.NET Community 2017 for server development
- Postgres 9.5 in an ubuntu VM on the same Win 10 PC
- pgAdmin for postgres admin
- VS Code for web and CLI development
- Android Studio for android app
- Postman for API runs

#### Deployed Environment Types
I don't describe the detail yet, but this runs on linux (Ubuntu 16) on ARM based HW for upper environments. Should run on full windows or other *nix combinations.

#### Database Setup
- Install postgres (9.5+) and pgAdmin
- Modify postgres config `pg_hba.conf` to allow LAN access. This example assumes a subnet of `198.168.0.x`. Add the following line to the config file using your subnet
    `host all all 192.168.0.0/24 md5`

- Add the following line to the postgres config `postgresql.conf` so postgres will listen for all clients on the network.
    `listen_addresses = '*'`



# Header
## Header 2
### Header 3
#### Header 4

**Section header**

Other Header
----

`inline highlighted`

- bullet
- s

* other 
* bullets

> Quote? type
> text

[link with address](www.google.com)

```sh
shell commands
usually preceed with 
$ ./pickem.py
```

| Column 1 | Column 2 |
| ------ | ------ |
| Value a | Value b |
| Value c | Value d |