#!/bin/bash

remote_pack_file_url="https://gist.githubusercontent.com/tommasobertoni/b6908c192edafe1e3a50151e0ad72ea6/raw/97c8946d7406a258d2da2e91ef3c11ed84a8e44b/pack.sh"
pack_file="pack.sh"

if ! test -f "$pack_file"; then
    echo "Downloading pack.sh..."
    wget -O $pack_file $remote_pack_file_url -q
fi

chmod a+x $pack_file
bash $pack_file -P "../src/IsAwaitable/IsAwaitable.csproj" -B -F "net5.0" -S -O "tommasobertoni" -L "../LICENSE"
