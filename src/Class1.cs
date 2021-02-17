using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace WinReg
{
    public class Class1
    {
        [DllExport]
        public static string reg(string arg_string)
        {
            if (String.IsNullOrEmpty(arg_string)) { return "OUT=base_key|sub_key|action(set_key,set_val,delete_key,delete_val,get_val,get_all_vals,get_subkeys)|data1|data2"; }

            string[] args = arg_string.Split('|');

            IDictionary<string, RegistryKey> system_keys = new Dictionary<string, RegistryKey>() {
                { "HKCR", Registry.ClassesRoot },
                { "HKCC", Registry.CurrentConfig },
                { "HKCU", Registry.CurrentUser },
                { "HKLM", Registry.LocalMachine },
                { "HKU", Registry.Users }
            };

            string[] valid_actions = { "set_key", "set_val", "delete_key", "delete_val", "get_val", "get_all_vals", "get_subkeys" };

            string return_value = "";
            string sub_key_str  = "";
            string action       = "";
            string data1        = "";
            string data2        = "";

            //Gotta fuckin assign them or it won't compile, even though it returns out if
            //it's not a valid key. Stupid fuckin compiler doesn't get shit lol. Pretty
            //sure we don't need to close the keys before using them cause you can't
            //close system keys.
            RegistryKey base_key = Registry.LocalMachine;
            RegistryKey sub_key  = Registry.LocalMachine;

            for (int i = 0; i < args.Length; i++) {
                if (i == 0) {
                    if (!system_keys.TryGetValue(args[i], out base_key)) { return_value = "ERR=Unable to open root key " + args[i] + "."; }
                } else if (i == 1) {
                    sub_key_str = args[i];
                } else if (i == 2) {
                    if (Array.IndexOf(valid_actions, args[i].ToLower()) > -1) {
                        action = args[i].ToLower();
                    } else {
                        return_value = "ERR=" + args[i] + " isn't a valid action.";
                    }
                } else if (i == 3) {
                    data1 = args[i];
                } else if (i == 4) {
                    data2 = args[i];
                } else {
                    return_value = "ERR=Excess amount of arguments; expected 5 arguments, recieved " + args.Length.ToString() + " args.";
                }
                if (!String.IsNullOrEmpty(return_value)) { break; }
            }

            if (String.IsNullOrEmpty(return_value)) { 
                if (args.Length == 5) {
                    try {
                        sub_key = base_key.OpenSubKey(sub_key_str, true);
                    }
                    catch (Exception e) { return_value = "ERR=" + e.ToString(); }
                    if (action == "set_key") {
                        try {
                            sub_key.CreateSubKey(data1);
                            return_value = "OUT=true";
                        } catch (Exception e) { return_value = "ERR=" + e.ToString(); }
                    } else if (action == "set_val") {
                        try {
                            sub_key.SetValue(data1, data2);
                            return_value = "OUT=true";
                        } catch (Exception e) { return_value = "ERR=" + e.ToString(); }
                    } else if (action == "delete_key") {
                        try {
                            sub_key.DeleteSubKey(data1);
                            return_value = "OUT=true";
                        } catch (Exception e) {
                            if (typeof(ArgumentException) == e.GetType()) {
                                return_value = "OUT=false";
                            } else {
                                return_value = "ERR=" + e.ToString();
                            }
                        }
                    } else if (action == "delete_val") {
                        try {
                            sub_key.DeleteValue(data1);
                            return_value = "OUT=true";
                        } catch (Exception e) {
                            if (typeof(ArgumentException) == e.GetType()) {
                                return_value = "OUT=false";
                            } else {
                                return_value = "ERR=" + e.ToString();
                            }
                        }
                    } else if (action == "get_val") {
                        object result = sub_key.GetValue(data1);
                        if (result != null) { return_value = "OUT=" + result.ToString(); } else { return_value = "OUT=nil"; }
                    } else if (action == "get_all_vals" ) {
                        string[] all_vals = sub_key.GetValueNames();
                        return_value = "OUT=";
                        foreach (string value in all_vals) { return_value += value + "|"; }
                    } else if (action == "get_subkeys") {
                        try {
                            string[] sub_key_names = sub_key.GetSubKeyNames();
                            return_value = "OUT=";
                            foreach (string name in sub_key_names) {
                                return_value += name + "|";
                            }
                        } catch (Exception e) { return_value = "ERR=" + e.ToString(); }
                    }
                } else {
                    return_value = "ERR=Missing required arguments; expected 5 arguments, recieved " + args.Length.ToString() + " args.";
                }
            }
            return return_value;
        }

    }
}
