# KerboresToMSSQL
**KerboresToMSSQL** 是一款轻量级工具，旨在使用当前域用户的本地 Kerberos 认证连接到 MSSQL 数据库。它不需要密码、用户名或哈希值，并允许在同一域内的任何数据库服务上执行命令查询。
该工具可以编译为独立的二进制文件，适用于通过web攻击进入内网环境具备用户shell权限但是没有凭证的数据库访问。
# 功能
1、使用当前域用户的 Kerberos 凭据连接 MSSQL 数据库
2、不需要密码或 NTLM 哈希
3、支持交互式 SQL Shell
4、支持使用查询文件的非交互模式
5、可轻松编译为独立二进制文件，便于携带和部署

# 使用方法
## 交互模式
使用以下命令启动交互式 SQL 查询 Shell：
```zsh
kerborestomssql.exe --server=dc.corp.com
```
该命令会使用当前用户的 Kerberos 凭证连接到 `dc.corp.com` 上的 MSSQL 服务器，并打开一个实时 SQL 查询 Shell。
## 非交互模式（基于文件执行）
你也可以使用文件中的 SQL 查询以非交互模式执行：
```zsh
kerborestomssql.exe --server=dc.corp.com --noshell-file=test.txt
```
`test.txt` 文件应该包含逐行排列的 SQL 查询语句，每条语句必须以分号 (`;`) 结尾。
**示例 `test.txt` 内容：**
```zsh
SELECT name FROM sys.databases;
SELECT user_name();
SELECT @@VERSION;
```
---
# KerboresToMSSQL
KerboresToMSSQL is a lightweight tool designed for connecting to MSSQL databases using the local Kerberos authentication of the current domain user. It requires no password, username, or hash, and allows command execution on any database service within the same domain.
This tool can be compiled into a standalone binary and is ideal for use within domain environments.
# Features
1,Connect to MSSQL databases using current domain user’s Kerberos ticket
2,No need for password or NTLM hash
3,Interactive SQL shell support
4,Non-interactive mode using query files
5,Easily compiled into a standalone binary for portability
# Usage
## Interactive Mode
Launch an interactive SQL shell with the following command:
```zsh
kerborestomssql.exe --server=dc.corp.com
```
This connects to the MSSQL server at dc.corp.com using the current user's Kerberos credentials and opens a live SQL query shell.
## Non-Interactive Mode (File-Based Execution)
You can also execute SQL queries from a file in a non-interactive session:
```zsh
kerborestomssql.exe --server=dc.corp.com --noshell-file=test.txt
```
test.txt should contain one SQL query per line.
Each query must be terminated with a semicolon (;).
Example test.txt:
```zsh
SELECT name FROM sys.databases;
SELECT user_name();
SELECT @@VERSION;
```
