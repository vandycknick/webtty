var pty = require("node-pty");

// PWSH
// var command = "pwsh";
// var args = ["-command", "Get-Date; ls; Get-Date; stty -a; tty"];
// var args = ["-command", "bash -c ls"];

// BASH
// var command = "bash";
// var args = ["-c", "ls"];

// NODE
var command = "node";
var args = ["-e", "require('fs'); console.log(fs.readdirSync(process.cwd()))"];

var ptyProcess = pty.spawn(command, args, {
    name: "xterm-color",
    cols: 80,
    rows: 30,
    cwd: process.cwd(),
    env: process.env,
});

ptyProcess.on("data", (data) => process.stdout.write(data));
