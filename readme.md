# CPack Package Manager
CPack is a fast, lightweight package manager for Visual C++. \
CPack stores information on a package in a JSON file format. End users install the package to a vcxproj file where its dependancies are added.

## Converting A Library to CPack
Navigate to the library folder and enter the command 'cpack init' \
Answer the basic questions about the library. This would create a CPack.json file in the library folder. \

## Installing A CPack Package
Navgigate to the library folder and enter 'cpack localize' this will localize all of the paths to your machine. \
Then to actually install the library enter cpack install
