# Standalone Input Reference

The executable generated by compiling C++ and QIR in this folder represents a reference on the behavior of QIR-based standalone executables. The goal is to have a sample that can be used as guidance to develop an entry-point generator which will be integrated to the Q# compiler.

The executable receives arguments through the command line. Each Q# data type that can be used as paramenter in an entry-point operation has a required option. Every parsed option is utilized to invoke the entry-point operation defined in the Q# source code (from which QIR is generated to the `qir-standalone-input-reference.ll` file). This operation just outputs the values of the received arguments through Q#'s `Message` API.

By default, the output produced during the simulation and the output returned by the entry-point operation are written to the standard output. To change where these outputs are written to, the executable also provides two non-required options to redirect them:
- **`-s`, `--simulation-output`**: File to write the output produced during the simulation (`Message` API invocations).
- **`-o`, `--operation-output`**: File to write the output of the Q# entry-point operation.


The `qir-driver.cpp` file contains the logic that parses the command line arguments using the [CLI11](https://github.com/CLIUtils/CLI11) library, and then invokes the entry point operation.
