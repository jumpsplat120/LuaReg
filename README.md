# LuaReg
 Basic Windows registry manipulation using lua.

## Installation
Drop the bin folder into your project. Feel free to name it whatever you'd like. `require` main.

## Dependencies
Written for LuaJIT. Needs ffi to load the dll, since I wrote the dll in C# and exported it using DllExport. Feel free to take a look at the .cs file and rewrite it to use the lua api. Uses one line from Love2D that helps locate the currently running project; `love.filesystem.getSource()`. If you can replace that, you don't need Love.

## Notes
Some terminology might not be what you expect. That's due to the term key originally being used to refer both to the registry key, as well as the key for a key/value pair within a registry key. So from here on, keys refer exclusively to the key/value pairs, while **entries** are the "folder" that holds the key/value pairs.

Most methods have a root and a path. A root is the top level entries, in their abbreviated form. For example, `HKEY_LOCAL_MACHINE` would be `HKLM`. Path is the rest of the path to an entry. Path's use blackslashes, and don't include the root. So for example, if you wanted the entry `HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft`, root would be `HKLM` and path would be `SOFTWARE\Microsoft`. Don't forget to escape your backslashes.

When working with luareg, keep WinReg.dll next to main.lua. If you are using love2d and you are fusing the file for distribution, move WinReg.dll to the top level, next to the executable and the rest of the love dll's. If you're not using love2d, then change the line in main.lua under reg:load to not use isFused, and use the second option in that tern.

## Usage
### reg:load()
This is used to load various things. This line also contains the only love2D call. If you want to use this project without Love, change the love line to be everything up to the working .lua. For example, if your dir looks like this
```
- main.lua
- luareg
  - main.lua
  - WinReg.dll
  - classic.lua
```
Then source should be everything up to the first main.lua. the subpath lines takes care of the rest.

### reg:deleteKey(root, path, key)
Delete a key/value pair from a registry entry. If the key does not exist, returns false, otherwise on a successful delete will return true.

### function reg:deleteEntry(root, path, key)
Delete an entry from the registry. If the entry does not exist, returns false, otherwise on a successful delete will return true.

### function reg:setKey(root, path, key, value)
Set a key/value pair for a registry entry. If the value already exists, overwrites the existing key/value pair. Currently only sets REG_SZ type values, mainly because I'm lazy and don't really know what or why there are different types for. Returns true on success. Feel free to make a pull request on the .cs for handling other registry data types.
	
### function reg:setEntry(root, path, key)
Set a registry entry. If the current registry entry already exists, then nothing happens. Returns true on success.

### function reg:getValue(root, path, key)
Gets a value from a key from a registry entry. If the key/value pair doesn't exist, then it returns nil, otherwise will return the value.

### function reg:getKeys(root, path)
Returns a table of all key names under a specific registry entry. If the entry has no key/value pairs, then returns an empty table.

### function reg:getSubEntries(root, path)
Returns a table of all sub entries under a specific root/path. Returns an empty table if there is nothing below this.
