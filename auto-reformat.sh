#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd ${DIR}

function reformatDir {
	for dir in "$1"/*;
	do
		if [ -d "$dir" ];
			then reformatDir "$dir";
		fi;
	done;
	for cs in "$1"/*.cs;
	do
		if [ -f "$cs" ];
			then
				dos2unix -U "$cs";
				if [ -n "`tail -1c \"$cs\"`" ];
					then echo "\
" >> "$cs";
				fi;
		fi;
	done;
	git commit -m "auto-reformat" "$1"/*.cs;
}

reformatDir "${DIR}"
