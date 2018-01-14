#!/bin/bash

if [ -f s ]; then
    rm s
	echo "removed previous server executable"
fi
g++-5 server_new.cpp -Wall -std=c++11 -o s
echo "compilation succesfull, the server will now start"
echo ""
./s
