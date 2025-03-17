using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.TRandomData {

    public static class RandomData {


        public static Color GetRandomColor() {
            return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);
        }

        private static int sequencialColorIndex = -1;
        private static Color[] sequencialColors = new[] {
            GetColorFromString("26a6d5"),
            GetColorFromString("41d344"),
            GetColorFromString("e6e843"),
            GetColorFromString("e89543"),
            GetColorFromString("0f6ad0"),
            GetColorFromString("b35db6"),
            GetColorFromString("c45947"),
            GetColorFromString("9447c4"),
            GetColorFromString("4756c4"),
        };

        public static void ResetSequencialColors() {
            sequencialColorIndex = -1;
        }

        public static Color GetSequencialColor() {
            sequencialColorIndex = (sequencialColorIndex + 1) % sequencialColors.Length;
            return sequencialColors[sequencialColorIndex];
        }

        public static Color GetColor255(float red, float green, float blue, float alpha = 255f) {
            return new Color(red / 255f, green / 255f, blue / 255f, alpha / 255f);
        }


        // Generate random normalized direction
        public static Vector3 GetRandomDir() {
            return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        }

        // Generate random normalized direction
        public static Vector3 GetRandomDirXZ() {
            return new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        }

        public static float GetAngleFromVectorFloat(Vector3 dir) {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }

        public static float GetAngleFromVectorFloatXZ(Vector3 dir) {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }

        public static int GetAngleFromVector(Vector3 dir) {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            int angle = Mathf.RoundToInt(n);

            return angle;
        }

        public static int GetAngleFromVector180(Vector3 dir) {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            int angle = Mathf.RoundToInt(n);

            return angle;
        }




        // Return random element from array
        public static T GetRandom<T>(T[] array) {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }

        // Returns a random script that can be used to id
        public static string GetIdString() {
            string alphabet = "0123456789abcdefghijklmnopqrstuvxywz";
            string ret = "";
            for (int i = 0; i < 8; i++) {
                ret += alphabet[UnityEngine.Random.Range(0, alphabet.Length)];
            }
            return ret;
        }

        // Returns a random script that can be used to id (bigger alphabet)
        public static string GetIdStringLong() {
            return GetIdStringLong(16);
        }

        // Returns a random script that can be used to id (bigger alphabet)
        public static string GetIdStringLong(int chars) {
            string alphabet = "0123456789abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ";
            string ret = "";
            for (int i = 0; i < chars; i++) {
                ret += alphabet[UnityEngine.Random.Range(0, alphabet.Length)];
            }
            return ret;
        }


        // Get a random male name and optionally single letter surname
        public static string GetRandomMaleName(bool withSurname = false) {
            List<string> firstNameList = new List<string>(){"Gabe","Cliff","Tim","Ron","Jon","John","Mike","Seth","Alex","Steve","Chris","Will","Bill","James","Jim",
                                        "Ahmed","Omar","Peter","Pierre","George","Lewis","Lewie","Adam","William","Ali","Eddie","Ed","Dick","Robert","Bob","Rob",
                                        "Neil","Tyson","Carl","Chris","Christopher","Jensen","Gordon","Morgan","Richard","Wen","Wei","Luke","Lucas","Noah","Ivan","Yusuf",
                                        "Ezio","Connor","Milan","Nathan","Victor","Harry","Ben","Charles","Charlie","Jack","Leo","Leonardo","Dylan","Steven","Jeff",
                                        "Alex","Mark","Leon","Oliver","Danny","Liam","Joe","Tom","Thomas","Bruce","Clark","Tyler","Jared","Brad","Jason"};

            if (!withSurname) {
                return firstNameList[UnityEngine.Random.Range(0, firstNameList.Count)];
            } else {
                string alphabet = "ABCDEFGHIJKLMNOPQRSTUVXYWZ";
                return firstNameList[UnityEngine.Random.Range(0, firstNameList.Count)] + " " + alphabet[UnityEngine.Random.Range(0, alphabet.Length)] + ".";
            }
        }

        // Get a random female name and optionally single letter surname
        public static string GetRandomFemaleName(bool withSurname = false) {
            List<string> firstNameList = new List<string>() {
                "Emily","Sophia","Olivia","Isabella","Ava","Mia","Amelia","Charlotte","Emma","Lily","Grace","Hannah","Abigail","Ella",
                "Scarlett","Chloe","Natalie","Samantha","Madison","Lucy","Aria","Layla","Zoe","Nora","Aurora","Stella","Violet","Hazel",
                "Ellie","Luna","Savannah","Leah","Aaliyah","Audrey","Brooklyn","Claire","Paisley","Penelope","Camila","Sofia","Harper",
                "Addison","Evelyn","Genesis","Skylar","Bella","Naomi","Quinn","Peyton","Alexa","Serenity","Caroline","Kennedy","Allison",
                "Aubrey","Ariana","Jasmine","Melanie","Kinsley","Valentina","Julia","Delilah","Riley","Autumn","Faith","Sarah","Nevaeh",
                "Gabriella","Lillian","Eliana","Maria"
            };

            if (!withSurname) {
                return firstNameList[UnityEngine.Random.Range(0, firstNameList.Count)];
            } else {
                string alphabet = "ABCDEFGHIJKLMNOPQRSTUVXYWZ";
                return firstNameList[UnityEngine.Random.Range(0, firstNameList.Count)] + " " + alphabet[UnityEngine.Random.Range(0, alphabet.Length)] + ".";
            }
        }

        public static string GetRandomName(bool withSurname = false) {
            if (TestChance(50, 100)) {
                return GetRandomMaleName(withSurname);
            } else {
                return GetRandomFemaleName(withSurname);
            }
        }



        public static string GetRandomCityName() {
            List<string> cityNameList = new List<string>(){"Alabama","New York","Old York","Bangkok","Lisbon","Vee","Agen","Agon","Ardok","Arbok",
                            "Kobra","House","Noun","Hayar","Salma","Chancellor","Dascomb","Payn","Inglo","Lorr","Ringu",
                            "Brot","Mount Loom","Kip","Chicago","Madrid","London","Gam",
                            "Greenvile","Franklin","Clinton","Springfield","Salem","Fairview","Fairfax","Washington","Madison",
                            "Georgetown","Arlington","Marion","Oxford","Harvard","Valley","Ashland","Burlington","Manchester","Clayton",
                            "Milton","Auburn","Dayton","Lexington","Milford","Riverside","Cleveland","Dover","Hudson","Kingston","Mount Vernon",
                            "Newport","Oakland","Centerville","Winchester","Rotary","Bailey","Saint Mary","Three Waters","Veritas","Chaos","Center",
                            "Millbury","Stockland","Deerstead Hills","Plaintown","Fairchester","Milaire View","Bradton","Glenfield","Kirkmore",
                            "Fortdell","Sharonford","Inglewood","Englecamp","Harrisvania","Bosstead","Brookopolis","Metropolis","Colewood","Willowbury",
                            "Hearthdale","Weelworth","Donnelsfield","Greenline","Greenwich","Clarkswich","Bridgeworth","Normont",
                            "Lynchbrook","Ashbridge","Garfort","Wolfpain","Waterstead","Glenburgh","Fortcroft","Kingsbank","Adamstead","Mistead",
                            "Old Crossing","Crossing","New Agon","New Agen","Old Agon","New Valley","Old Valley","New Kingsbank","Old Kingsbank",
                "New Dover","Old Dover","New Burlington","Shawshank","Old Shawshank","New Shawshank","New Bradton", "Old Bradton","New Metropolis","Old Clayton","New Clayton"
            };
            return cityNameList[UnityEngine.Random.Range(0, cityNameList.Count)];
        }

        public static Vector3 GetRandomPositionWithinRectangle(float xMin, float xMax, float yMin, float yMax) {
            return new Vector3(UnityEngine.Random.Range(xMin, xMax), UnityEngine.Random.Range(yMin, yMax));
        }

        public static Vector3 GetRandomPositionWithinRectangleXZ(float xMin, float xMax, float zMin, float zMax) {
            return new Vector3(UnityEngine.Random.Range(xMin, xMax), 0, UnityEngine.Random.Range(zMin, zMax));
        }

        public static Vector3 GetRandomPositionWithinRectangle(Vector3 lowerLeft, Vector3 upperRight) {
            return new Vector3(UnityEngine.Random.Range(lowerLeft.x, upperRight.x), UnityEngine.Random.Range(lowerLeft.y, upperRight.y));
        }

        public static bool TestChance(int chance, int chanceMax = 100) {
            return UnityEngine.Random.Range(0, chanceMax) < chance;
        }






        // Returns 00-FF, value 0->255
        public static string Dec_to_Hex(int value) {
            return value.ToString("X2");
        }

        // Returns 0-255
        public static int Hex_to_Dec(string hex) {
            return Convert.ToInt32(hex, 16);
        }

        // Returns a hex string based on a number between 0->1
        public static string Dec01_to_Hex(float value) {
            return Dec_to_Hex((int)Mathf.Round(value * 255f));
        }

        // Returns a float between 0->1
        public static float Hex_to_Dec01(string hex) {
            return Hex_to_Dec(hex) / 255f;
        }

        // Get Hex Color FF00FF
        public static string GetStringFromColor(Color color) {
            string red = Dec01_to_Hex(color.r);
            string green = Dec01_to_Hex(color.g);
            string blue = Dec01_to_Hex(color.b);
            return red + green + blue;
        }

        // Get Hex Color FF00FFAA
        public static string GetStringFromColorWithAlpha(Color color) {
            string alpha = Dec01_to_Hex(color.a);
            return GetStringFromColor(color) + alpha;
        }

        // Sets out values to Hex String 'FF'
        public static void GetStringFromColor(Color color, out string red, out string green, out string blue, out string alpha) {
            red = Dec01_to_Hex(color.r);
            green = Dec01_to_Hex(color.g);
            blue = Dec01_to_Hex(color.b);
            alpha = Dec01_to_Hex(color.a);
        }

        // Get Hex Color FF00FF
        public static string GetStringFromColor(float r, float g, float b) {
            string red = Dec01_to_Hex(r);
            string green = Dec01_to_Hex(g);
            string blue = Dec01_to_Hex(b);
            return red + green + blue;
        }

        // Get Hex Color FF00FFAA
        public static string GetStringFromColor(float r, float g, float b, float a) {
            string alpha = Dec01_to_Hex(a);
            return GetStringFromColor(r, g, b) + alpha;
        }

        // Get Color from Hex string FF00FFAA
        public static Color GetColorFromString(string color) {
            float red = Hex_to_Dec01(color.Substring(0, 2));
            float green = Hex_to_Dec01(color.Substring(2, 2));
            float blue = Hex_to_Dec01(color.Substring(4, 2));
            float alpha = 1f;
            if (color.Length >= 8) {
                // Color string contains alpha
                alpha = Hex_to_Dec01(color.Substring(6, 2));
            }
            return new Color(red, green, blue, alpha);
        }

        public static T GetRandomElement<T>(List<T> list) {
            if (list.Count == 0) {
                return default(T);
            } else {
                return list[UnityEngine.Random.Range(0, list.Count)];
            }
        }

        public static T GetRandomElement<T>(T[] array) {
            if (array.Length == 0) {
                return default(T);
            } else {
                return array[UnityEngine.Random.Range(0, array.Length)];
            }
        }

    }


    public static class RandomDataExtensions {


        public static T GetRandomElement<T>(this List<T> list) {
            if (list.Count == 0) {
                return default(T);
            } else {
                return list[UnityEngine.Random.Range(0, list.Count)];
            }
        }

        public static T GetRandomElement<T>(this T[] array) {
            if (array.Length == 0) {
                return default(T);
            } else {
                return array[UnityEngine.Random.Range(0, array.Length)];
            }
        }

    }

    }