#!/bin/bash
current_dir=$(dirname "$0")
"$SPLUNK_HOME/bin/splunk" cmd /usr/bin/mono "$current_dir/random-numbers.exe" $@ 
