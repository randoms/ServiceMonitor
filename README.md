# ServiceMonitor
a simple service monitor program, sends email to you when your service is down.

## Configuration
set the values in app.conf file.

```
<!-- Email发送的地址 -->
<add key="FromEmail" value="xxx@xxx"/> 
<!-- Email发送的用户名 -->
<add key="Username" value="xxx"/>
<!-- Email发送的密码 -->
<add key="Password" value="xxx"/>
<!-- Email接收的地址 -->
<add key="ToEmail" value="xxx@xxx"/>
<!-- Email发送服务器域名 -->
<add key="Domain" value="xxx"/>
<!-- Email发送服务器端口 -->
<add key="Port" value="587"/>
<!-- 需要监控的服务的地址，以逗号隔开 -->
<add key="Services" value="www.xx.com,www.xxx.com,www.xxxxx.com"/>
```

## Screenshots
当服务挂掉的时候

![Service Down](http://www.bwbot.org/static/img/servicedown.png)

当服务恢复的时候

![Service Down](http://www.bwbot.org/static/img/serviceup.png)
