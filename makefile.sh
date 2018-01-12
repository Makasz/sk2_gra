#!/bin/bash

g++-5 server.cpp -Wall -std=c++11 -o s
echo "compilation succesfull, the server will now start"
echo ""
./s
