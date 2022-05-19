using FlameInterpreter;

string source = File.ReadAllText("./program.flame");

var it = new Interpreter();
it.Run(source);