# PingSample
ASP.NET Core 8.0 PingSample

Verifies IP-level connectivity to another TCP/IP computer by sending Internet Control Message Protocol (ICMP) echo Request messages. The receipt of the corresponding echo Reply messages is displayed, along with round-trip times. ping is the primary TCP/IP command used to troubleshoot connectivity, reachability, and name resolution. Used without parameters, this command displays Help content.

You can also use this command to test both the computer name and the IP address of the computer. If pinging the IP address is successful, but pinging the computer name isn't, you might have a name resolution problem. In this case, make sure the computer name you're specifying can be resolved through the local Hosts file, by using Domain Name System (DNS) queries, or through NetBIOS name resolution techniques.

```sh
ping [/t] [/a] [/n <count>] [/l <size>] [/f] [/I <TTL>] [/v <TOS>] [/r <count>] [/s <count>] [{/j <hostlist> | /k <hostlist>}] [/w <timeout>] [/R] [/S <Srcaddr>] [/4] [/6] <targetname>
```


## example

```sh
ping google.com
```

## Custom ping payloads on Linux

### Source
- [Breaking change: Custom ping payloads on Linux - .NET | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/core/compatibility/networking/7.0/ping-custom-payload-linux) - Custom ping payloads on Linux

### Previous behavior
In previous versions, the ping packet payload was silently ignored (that is, it wasn't sent) on non-privileged Linux processes.

### New behavior
Starting in .NET 7, a PlatformNotSupportedException is thrown if you attempt to send a custom ping packet payload when running in non-privileged Linux process.

### Version introduced
.NET 7

### Type of breaking change
This change can affect binary compatibility.

### Reason for change
It's better to signal to the user that the operation cannot be performed instead of silently dropping the payload.

### Recommended action
If a ping payload is necessary, run the application as root, or grant the cap_net_raw capability using the setcap utility.

Otherwise, use an overload of Ping.SendPingAsync that does not accept a custom payload, or pass in an empty array.

