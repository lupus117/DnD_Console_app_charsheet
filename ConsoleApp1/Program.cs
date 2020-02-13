using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WenceyWang.FIGlet;
namespace ConsoleApp1
{
    class Program
    {
        public static CharacterSheet sheet;
        public static List<Spell> spells = new List<Spell>();
        static void Main(string[] args)
        {
            sheet = new CharacterSheet();


            bool keepLooping = true;

            do
            {
                PrintSheet();

                keepLooping = QueryMain();
                Console.Clear();


            } while (keepLooping);
            Console.Clear();
            Console.WriteLine("thank you");
            Console.ReadKey(true);
        }
        private static bool QueryMain()
        {
            Console.WriteLine("Press [x] to exit, [e] to edit sheet, [f] to wiew features \n[s] to save a sheet and [l] to load it \n[m] for the spell menu");
            char press = Console.ReadKey(true).KeyChar;

            if (press == 'x' || press == 'q')
                return false;

            if (press == 'e')
                Edit();
            if (true)
            {
                int hp;
                if (press == '+')
                {
                    Console.Write("+");
                    if (int.TryParse(Console.ReadLine(), out hp))
                        sheet.Hp_hpmax[0] += hp;
                }


                if (press == '-')
                {
                    Console.Write("-");
                    if (int.TryParse(Console.ReadLine(), out hp))
                        sheet.Hp_hpmax[0] -= hp;
                }

            }
            if (press == 'f')
            {
                PrintFeatures();
                Console.ReadKey(true);
            }
            if (press == 's')
                SaveSheet();
            if (press == 'l')
                LoadSheet();
            if (press == 'm')
                SpellMode();
            if (press == '/')
            {
                Console.Write("/");
                Roll(Console.ReadLine());
            }

            return true;
        }
        private static void Roll(string input)
        {
            Random rnd = new Random();

            int amount;
            int die;
            int mod;
            string tmpamount = "";
            string tmpdie = "";
            string tmpmod = "";
            bool changed = false;
            bool modding = false;
            char function = ' ';
            foreach (char c in input)
            {
                if (char.IsNumber(c) && !changed && !modding)
                {
                    tmpamount += c;
                }
                else if (c == 'd' && changed != true && !modding || c == 'D' && changed != true && !modding)
                    changed = true;
                else if (char.IsNumber(c) && changed && !modding)
                {
                    tmpdie += c;
                }
                else if (!char.IsLetter(c) && !modding && !char.IsNumber(c) || c == 'x')
                {
                    function = c;
                    modding = true;
                }
                else if (char.IsNumber(c) && modding)
                {
                    tmpmod += c;
                }
                else return;
            }

            if (int.TryParse(tmpamount, out amount))
            {
                if (int.TryParse(tmpdie, out die))
                {
                    if (die < 1 && amount < 1)
                    {
                        return;
                    }
                    //this is where I left
                    Console.WriteLine($"Rolling: {amount} d{die}-s");

                    List<int> rolls = new List<int>();
                    for (int i = 0; i < amount; i++)
                    {

                        rolls.Add(rnd.Next(1, die + 1));
                    }
                    int sum = 0;

                    rolls.ForEach(a =>
                    {

                        Console.Write("[");
                        ColorfulCrits(a.ToString(), "1", die.ToString());
                        Console.Write($"] ");

                        sum += a;
                    });
                    Console.WriteLine($"\nTotal is :{sum}");


                    if (int.TryParse(tmpmod, out mod))
                    {
                        Console.WriteLine($"{function}{mod}");
                        if (function == '+')
                            sum += mod;
                        else
                        if (function == '-')
                            sum -= mod;
                        else
                        if (function == '*' || function == 'x')
                            sum *= mod;
                        else
                        if (function == '/')
                            sum /= mod;
                        else return;


                        Console.WriteLine("The total after modifiers is :" + sum);

                    }


                }
                else return;
            }
            else return;
            Console.ReadKey(true);
        }
        private static void LoadSheet()
        {
            Console.Clear();
            DirectoryInfo d = new DirectoryInfo(Path.GetFullPath(@"."));
            FileInfo[] Files = d.GetFiles("*.json");
            string str = "";
            int count = 1;
            Console.WriteLine($"Current json files to load:\n[");
            foreach (FileInfo file in Files)
            {
                if (count % 2 == 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                Console.WriteLine($"[{count}]{file.Name}");
                Console.ForegroundColor = ConsoleColor.White;
                count++;
            }
            Console.WriteLine("]");

            Console.WriteLine("Write a filename to load");

            string filename = Console.ReadLine();
            if (int.TryParse(filename, out int n) && 0 < n && n <= Files.Length)
            {
                filename = Files[n].Name;
            }
            else
            {
                if (!filename.Contains(".json"))
                {
                    filename = @"" + filename + ".json";
                }
                filename = @"" + filename;
            }

            if (File.Exists(filename))
            {
                string Data = File.ReadAllText(filename);
                if (Data != null)
                {
                    sheet = new CharacterSheet();
                    sheet.skills.Clear();
                    CharacterSheet _tmpsheet = JsonConvert.DeserializeObject<CharacterSheet>(Data);
                    int max = _tmpsheet.skills.Count;
                    sheet = _tmpsheet;

                    for (int i = max - 1; i >= 0; i--)
                    {
                        if (i < (max / 2))
                        {
                            sheet.skills.Remove(sheet.skills[i]);
                        }

                    }
                }
                else
                {
                    Console.WriteLine("File is empty");
                    Console.ReadKey(true);

                }
            }
            else
            {
                Console.WriteLine("File does not exist");
                Console.ReadKey(true);
            }

        }

        private static void ColorfulCrits(string input, string red, string green)
        {
            if (input == red)
            {
                Console.BackgroundColor = ConsoleColor.Red;
            }
            if (input == green)
            {
                Console.BackgroundColor = ConsoleColor.Green;

            }
            if (input == green && input == red)
            {
                Console.BackgroundColor = ConsoleColor.Blue;

            }
            Console.Write(input);
            Console.BackgroundColor = ConsoleColor.Black;

        }
        private static void SaveSheet()
        {
            Console.WriteLine("Write a filename to save");

            string filename = Console.ReadLine();
            filename = @"" + filename + ".json";
            string jsonSheet = JsonConvert.SerializeObject(sheet);
            System.IO.File.WriteAllText(filename, jsonSheet);
        }
        private static List<Spell> BulkSpell(string condition = "")
        {
            List<Spell> _searched = new List<Spell>();
            string input = condition;
            List<string[]> _conditions = new List<string[]>();
            if (input == "")
            {
                Console.WriteLine($"Enter the conditions you want to search by\nExample:\nlevel:cantrip;school:evocation;class:wizard;component:verbal somatic\n");
                input = Console.ReadLine();

            }
            _conditions = WordsToArrList(StringToList(input, ';'));

            try
            {


                foreach (var spell in spells)
                {
                    bool valid = true;

                    _conditions.ForEach(_con =>
                    {
                        if (_con[0] == "level" && spell.level.ToLower() != _con[1])
                        {
                            valid = false;
                        }

                        if (_con[0] == "casting_time" && spell.casting_time.ToLower() != _con[1])
                        {
                            valid = false;

                        }
                        if (_con[0] == "duration" && spell.duration.ToLower() != _con[1])
                        {
                            valid = false;

                        }
                        if (_con[0] == "range" && spell.range.ToLower() != _con[1])
                        {
                            valid = false;

                        }
                        if (_con[0] == "school" && spell.school.ToLower() != _con[1])
                        {
                            valid = false;

                        }
                        if (_con[0] == "name" && spell.name.ToLower() != _con[1])
                        {
                            valid = false;

                        }
                        if (_con[0] == "class" && !spell.tags.Contains(_con[1]))
                        {
                            valid = false;

                        }
                        if (_con[0] == "component")
                        {
                            if (_con[1].Contains("somatic"))
                            {
                                if (!spell.components.somatic)
                                {
                                    valid = false;
                                }
                            }
                            else if (spell.components.somatic)
                            {
                                valid = false;
                            }

                            if (_con[1].Contains("verbal"))
                            {
                                if (!spell.components.verbal)
                                {
                                    valid = false;
                                }
                            }
                            else if (spell.components.verbal)
                            {
                                valid = false;
                            }

                            if (_con[1].Contains("material"))
                            {
                                if (!spell.components.material)
                                {
                                    valid = false;
                                }
                            }
                            else if (spell.components.material)
                            {
                                valid = false;
                            }


                        }


                    });
                    if (valid)
                    {
                        _searched.Add(spell);
                    }
                }
            }
            catch
            {
                return new List<Spell>();
            }

            _searched.ForEach(a => Console.WriteLine($"name: {a.name}\n level:{a.level} \nschool:{a.school} "));
            return _searched;
        }
        private static List<string[]> WordsToArrList(List<string> list, char sepperator = ':')
        {
            List<string[]> response = new List<string[]>();
            list.ForEach(a =>
            {
                string[] tmp = a.Split(sepperator);
                response.Add(tmp);
            });
            return response;
        }
        private static List<string> StringToList(string input, char seperator)
        {
            string _string = input.ToLower() + seperator;
            List<string> _responce = new List<string>();
            string tmpword = "";
            foreach (char c in _string)
            {
                if (c != seperator)
                {
                    tmpword += c;
                }
                if (c == seperator)
                {
                    _responce.Add(tmpword);
                    tmpword = "";
                }


            }
            return _responce;
        }

        private static List<string> StringToWords(string input)
        {
            string _string = input.ToLower() + " ";
            List<string> _responce = new List<string>();
            string tmpword = "";
            bool isWord = false;
            foreach (char c in _string)
            {
                if (c != ' ')
                {
                    if (!isWord)
                    {
                        isWord = true;

                        tmpword = "";
                        tmpword += c;
                    }
                    else
                    {
                        tmpword += c;

                    }
                }
                else
                {
                    if (isWord)
                    {
                        _responce.Add(tmpword);
                        isWord = false;
                    }
                }

            }
            return _responce;
        }
        private static void PrintSpell(string spellname)
        {
            Spell _spell;
            string _spellname = spellname.ToLower();
            _spell = spells.FirstOrDefault(a => a.name.ToLower() == (_spellname));
            if (_spell == null)
                _spell = spells.FirstOrDefault(a => a.name.ToLower().Contains(_spellname));
            if (_spell != null)
            {
                Console.Clear();
                AsciiArt askii = new AsciiArt(_spell.name);
                Console.WriteLine(askii + "\n");
                Console.WriteLine("Level: {0,7} {1}", _spell.level, _spell.school);
                string tmpcomponents = "";
                if (_spell.components.somatic) tmpcomponents += "Somatic ";
                if (_spell.components.material) tmpcomponents += "Material ";
                if (_spell.components.verbal) tmpcomponents += "Verbal ";



                Console.WriteLine($"Components: {tmpcomponents}");
                if (_spell.components.material) _spell.components.materials_needed.ForEach(a => Console.WriteLine(a));

                Console.WriteLine($"\nRange : {_spell.range} \nDuration: {_spell.duration}\n");
                PrintThinLines($"{_spell.description}\n\n{_spell.higher_levels}", 70);

                Console.WriteLine("\nClasses: \n");
                _spell.tags.ForEach(a => Console.WriteLine(a));

                Console.ReadKey(true);
            }
        }
        private static void SpellMode()
        {
            List<Spell> _tmpSpells = new List<Spell>();
            if (sheet.spellsKnown != null)
            {
                _tmpSpells = sheet.spellsKnown;
            }
            Console.Clear();
            string path = @"spells";
            if (File.Exists(path))
            {
                Console.WriteLine("Loading Data...");
                string data = File.ReadAllText(path);
                spells = JsonConvert.DeserializeObject<List<Spell>>(data);
                spells.ForEach(a => Console.WriteLine($"Found spell {a.level}: {a.name}"));
                char c;
                do
                {

                    Console.Clear();
                    _tmpSpells = Sortbylevel(_tmpSpells);
                    int count = 1;
                    _tmpSpells.ForEach(a =>
                    {
                        if (count % 2 == 0)
                            Console.ForegroundColor = ConsoleColor.DarkGray;

                        Console.WriteLine("[{0,7}|{1,30}|{2,15}]        nr:{3}", a.level, a.name, a.school, _tmpSpells.FindIndex(x => x == a));
                        Console.ForegroundColor = ConsoleColor.White;
                        count++;

                    });
                    Console.WriteLine("\n Press [s] to save a spells to sheet \n[x] to exit \n[c] to clear spells \n[/] to view a spell from name or nr \n[b] to add spells by filter");

                    c = Console.ReadKey(true).KeyChar;
                    if (c == 'b')
                    {
                        List<Spell> _spells = BulkSpell();
                        if (_spells != null)
                        {
                            _spells.ForEach(a =>
                            {
                                if (!_tmpSpells.Contains(a))
                                {
                                    _tmpSpells.Add(a);

                                }
                            });

                        }
                    }
                    if (c == 'c')
                    {

                        _tmpSpells.Clear();
                    }
                    if (c == '/')
                    {
                        Console.Write("Enter spell name: ");
                        string s = Console.ReadLine();
                        if (int.TryParse(s, out int n) && _tmpSpells.Count > n)
                        {
                            PrintSpell(_tmpSpells[n].name);
                        }
                        else
                            PrintSpell(s);
                    }
                    if (c == 'l')
                    {
                        Sortbylevel(BulkSpell()).ForEach(a => PrintSpell(a.name));

                    }
                    if (c == 's')
                    {

                        sheet.spellsKnown = _tmpSpells;
                    }
                    if (c == '-')
                    {
                        Console.Write("\nSpell(s) to remove from list(seperated by ; if many \n-");
                        string spellnames = Console.ReadLine();
                        foreach (string s in spellnames.Split(';'))
                        {
                            Spell _spell;
                            if (int.TryParse(s, out int n) && _tmpSpells.Count > n)
                            {
                                _spell = _tmpSpells[n];
                            }
                            else
                                _spell = _tmpSpells.Find(a => a.name == s);
                            if (_spell != null)
                            {
                                _tmpSpells.Remove(_spell);
                            }
                        }
                    }
                    if (c == '+')
                    {
                        Console.Write("\nSpell(s) to add (seperated by ; if many) \n+");
                        string spellnames = Console.ReadLine();
                        foreach (string s in spellnames.Split(';'))
                        {
                            Spell _spell = spells.First(a => a.name.ToLower() == s.ToLower());
                            if (_spell != null)
                            {
                                _tmpSpells.Add(_spell);

                            }
                        }
                    }
                } while (c != 'x');
            }
            else
            {
                Console.WriteLine("Spell file [spells.json] not located");
            }
        }
        private static List<Spell> Sortbylevel(List<Spell> spellList)
        {
            List<Spell> _spellList = new List<Spell>();
            spellList.ForEach(a =>
                {
                    if (a.level == "cantrip")
                    {
                        _spellList.Add(a);
                    }
                });
            for (int i = 1; i < 10; i++)
            {
                spellList.ForEach(a =>
                {
                    if (a.level == i.ToString())
                    {
                        _spellList.Add(a);
                    }
                });
            }
            return _spellList;
        }
        private static void PrintThinLines(string input, int charAmount = 50)
        {
            int count = 0;

            foreach (char c in input)
            {
                Console.Write(c);
                count++;
                if (count % charAmount == 0)
                {
                    Console.WriteLine();
                }

            }
        }
        private static void PrintFeatures()
        {
            foreach (var feature in sheet.features)
            {
                Console.WriteLine(feature.Key + "\n");
                PrintThinLines(feature.Value, 50);
                Console.WriteLine("\n\n");
            }
        }
        private static void Edit()
        {
            Console.Clear();
            Console.WriteLine("press [key] to edit \n[s] Statistics \n[h] Hp, lvl and ac \n[k] Skills \n[f] features\n[t] for Saving Throws\n\t[x] Exit");
            char press = Console.ReadKey(true).KeyChar;

            if (press == 'x')
                return;
            if (press == 's')
                EditStats();
            if (press == 'h')
                EditHpLvlAC();
            if (press == 'k')
                EditSkills();
            if (press == 'f')
                EditFeatures();
            if (press == 'j')
            {
                if (sheet.JackOfAllTrades)
                    sheet.JackOfAllTrades = false;
                else
                    sheet.JackOfAllTrades = true;

            }
            if (press == 't')
            {
                EditSaves();
            }



        }
        private static void EditHpLvlAC()
        {
            Console.Clear();
            Console.WriteLine($"Hp :{sheet.Hp_hpmax[0]}/{sheet.Hp_hpmax[1]} lvl: {sheet.lvl} Proficiency bonus: [{ProfBonus()}]\n");

            Console.WriteLine("\nPress [key] to edit \n[h] hp \n[m] max hp \n[l] level \n[a] ac \n[n] name");
            char press = Console.ReadKey(true).KeyChar;
            Console.WriteLine("Enter value");
            if (press == 'h')
                int.TryParse(Console.ReadLine(), out sheet.Hp_hpmax[0]);
            if (press == 'm')
                int.TryParse(Console.ReadLine(), out sheet.Hp_hpmax[1]);
            if (press == 'l')
                int.TryParse(Console.ReadLine(), out sheet.lvl);
            if (press == 'a')
                int.TryParse(Console.ReadLine(), out sheet.AC);
            if (press == 'n')
            {
                Console.Write("Enter a name: ->");
                sheet.Name = Console.ReadLine();
            }


        }
        private static void EditStats()
        {

            Console.WriteLine("Which stat do you wish to edit?\n");
            foreach (var stat in sheet.stats)
            {
                Console.WriteLine("{0,20} : {1}", stat.Key, stat.Value);

            }
            Console.Write("Stat: -> ");
            string statToEdit = Console.ReadLine();
            foreach (var stat in sheet.stats)
            {
                if (stat.Key == statToEdit)
                {
                    Console.WriteLine("\nWhat a value do you want?");
                    if (int.TryParse(Console.ReadLine(), out int i))
                    {
                        sheet.stats[statToEdit] = i;
                        break;
                    }
                }

            }


        }
        private static void EditSaves()
        {
            Console.Clear();
            Console.WriteLine("Current save proficiencies:");
            foreach (var stat in sheet.stats)
            {

                if (sheet.savingThrows.Contains(stat.Key))
                {

                    Console.WriteLine("{0,20} : {1} [{2}] save:[{3}] ", stat.Key, stat.Value, StatMod(stat.Value), StatMod(stat.Value) + ProfBonus());


                }
                else
                {
                    Console.WriteLine("{0,20} : {1}[{2}] unproficient", stat.Key, stat.Value, StatMod(stat.Value), StatMod(stat.Value));
                }


            }
            Console.WriteLine("\npress [a] to add or [d] to delete");
            char b = Console.ReadKey(true).KeyChar;
            if (b == 'a')
            {
                Console.Write(" Enter the Statistic you want proficiency in ->");
                string str = Console.ReadLine();
                if (sheet.stats.ContainsKey(str))
                {
                    sheet.savingThrows.Add(str);
                }
            }
            if (b == 'd')
            {
                Console.Write(" Enter the Statistic no longer have proficiency in ->");
                string str = Console.ReadLine();
                if (sheet.savingThrows.Contains(str))
                {
                    sheet.savingThrows.Remove(str);
                }
            }


        }
        private static void EditSkills()
        {

            Console.WriteLine("Which skill do you wish to change proficiency?\n");
            foreach (var skill in sheet.skills)
            {
                Console.WriteLine("{0,20} : Mod[{1,2}] {2}", skill.Name, SkillMod(skill), skill.Prof);

            }
            Console.Write("Skill: -> ");
            string skillToEdit = Console.ReadLine();
            foreach (var skill in sheet.skills)
            {
                if (skill.Name == skillToEdit)
                {
                    Console.WriteLine("press a [key] and enter to set the proficiency value \n[p] proficient \n[e] Expertise \n a number for custom modifiers");
                    string ProfValue = Console.ReadLine();

                    if (ProfValue.ToLower() == "p")
                    {
                        sheet.skills.Find(a => a.Name == skill.Name).Prof = "Proficient";
                    }
                    else
                    if (ProfValue.ToLower() == "e")
                    {
                        sheet.skills.Find(a => a.Name == skill.Name).Prof = "Expertise";
                    }
                    else
                    if (int.TryParse(ProfValue, out int i))
                    {

                        sheet.skills.Find(a => a.Name == skill.Name).Prof = i.ToString();

                        break;
                    }
                    else sheet.skills.Find(a => a.Name == skill.Name).Prof = "";




                }

            }
        }
        private static void EditFeatures()
        {
            Console.Clear();
            PrintFeatures();

            Console.WriteLine("press [a] to add, [e] to edit or [d] to delete");
            char c = Console.ReadKey(true).KeyChar;

            if (c == 'e')
            {
                Console.WriteLine("Which stat do you wish to edit?\n");
                PrintFeatures();
                Console.Write("Feature: -> ");
                string statToEdit = Console.ReadLine();
                foreach (var stat in sheet.features)
                {
                    if (stat.Key == statToEdit)
                    {
                        Console.WriteLine("\nWhat a Value do you want?");
                        string s = Console.ReadLine();
                        if (s != "")
                        {
                            sheet.features[statToEdit] = s;
                            break;
                        }
                    }

                }
            }
            else if (c == 'a')
            {

                Console.WriteLine("Enter the name of the feature");
                string name = Console.ReadLine();
                Console.WriteLine("Enter the content of the feature");
                string content = Console.ReadLine();
                if (name != "" && content != "")
                {
                    sheet.features.Add(name, content);
                }

            }
            if (c == 'd')
            {
                Console.WriteLine("Which stat do you wish to delete?\n");
                Console.Write("Feature: -> ");
                string statToEdit = Console.ReadLine();
                foreach (var stat in sheet.features)
                {
                    if (stat.Key == statToEdit)
                    {
                        sheet.features.Remove(stat.Key);
                    }

                }
            }
        }
        private static void PrintSheet()
        {
            // string tmpSheet = "";
            Console.WriteLine($"Name:{sheet.Name}");
            Console.WriteLine($"Hp :{sheet.Hp_hpmax[0]}/{sheet.Hp_hpmax[1]} ({Math.Floor((double)(sheet.Hp_hpmax[0]) / (double)(sheet.Hp_hpmax[1]) * 100)}%) lvl: {sheet.lvl} Proficiency bonus: [{ProfBonus()}] AC:{sheet.AC}\n");

            Console.WriteLine("Stats:");

            foreach (var stat in sheet.stats)
            {
                // sheet.stats[stat.Key] = 10;
                if (sheet.savingThrows.Contains(stat.Key))
                {
                    Console.WriteLine("{0,20} : {1} [{2}] save:[{3}] ", stat.Key, stat.Value, StatMod(stat.Value), Convert.ToString(StatMod(stat.Value) + ProfBonus()));
                }
                else
                    Console.WriteLine("{0,20} : {1} [{2}] ", stat.Key, stat.Value, StatMod(stat.Value));

                //tmpSheet += ("\n{ 0,12}:{1}", stat.Key, stat.Value);
            }
            Console.WriteLine("\n Skills :");
            if (sheet.JackOfAllTrades)
            {
                Console.WriteLine(" Jack of all trades(add half of proficiency bonus to unproficient skills)");
            }
            foreach (var skill in sheet.skills)
            {
                Console.WriteLine("{0,20} | [{1}] {2}", skill.Name, SkillMod(skill), skill.Prof);
            }

        }
        private static int StatMod(int stat)
        {
            int mod = (int)Math.Floor((Convert.ToDecimal(stat.ToString()) - 10) / 2);
            return mod;
        }
        private static int SkillMod(Skills skill)
        {

            if (skill.Prof == "Proficient")
            {
                return StatMod(sheet.stats[skill.Stat]) + ProfBonus();
            }
            if (skill.Prof == "Expertise")
            {
                return StatMod(sheet.stats[skill.Stat]) + (2 * ProfBonus());
            }
            if (int.TryParse(skill.Prof, out int i))
            {
                if (sheet.JackOfAllTrades)
                {
                    return StatMod(sheet.stats[skill.Stat]) + i + (int)Math.Floor(Convert.ToDecimal(ProfBonus().ToString()) / 2);

                }
                else
                    return StatMod(sheet.stats[skill.Stat]) + i;
            }
            if (sheet.JackOfAllTrades)
            {
                return StatMod(sheet.stats[skill.Stat]) + (int)Math.Floor(Convert.ToDecimal(ProfBonus().ToString()) / 2);

            }
            else
                return StatMod(sheet.stats[skill.Stat]);


        }
        private static int ProfBonus()
        {
            return (int)Math.Floor((Convert.ToDecimal(sheet.lvl.ToString()) + 7) / 4);
        }

    }
    public class Skills
    {
        public string Name { get; set; }
        public string Stat { get; set; }
        public string Prof { get; set; }

    }
    public class CharacterSheet
    {
        public string Name;
        public bool JackOfAllTrades = false;
        public Dictionary<string, int> stats;
        public List<Skills> skills;
        public List<Spell> spellsKnown;
        public int[] Hp_hpmax = new int[2];
        public int lvl;
        public int AC;
        public Dictionary<string, string> features;
        public List<string> savingThrows;

        public
        CharacterSheet()
        {
            spellsKnown = new List<Spell>();
            features = new Dictionary<string, string>();
            skills = new List<Skills>();
            stats = new Dictionary<string, int>();
            savingThrows = new List<string>();

            if (stats != null)
            {
                stats.Add("Strength", 10);
                stats.Add("Dexterity", 10);
                stats.Add("Constitution", 10);
                stats.Add("Intelligence", 10);
                stats.Add("Wisdom", 10);
                stats.Add("Charisma", 10);
            }

            if (skills != null)
            {
                skills.Add(new Skills { Name = "Athletics", Stat = "Strength", Prof = "" });
                skills.Add(new Skills { Name = "Acrobatics", Stat = "Dexterity", Prof = "" });
                skills.Add(new Skills { Name = "Sleight of Hand", Stat = "Dexterity", Prof = "" });
                skills.Add(new Skills { Name = "Stealth", Stat = "Dexterity", Prof = "" });
                skills.Add(new Skills { Name = "Arcana", Stat = "Intelligence", Prof = "" });
                skills.Add(new Skills { Name = "History", Stat = "Intelligence", Prof = "" });
                skills.Add(new Skills { Name = "Investigation", Stat = "Intelligence", Prof = "" });
                skills.Add(new Skills { Name = "Nature", Stat = "Intelligence", Prof = "" });
                skills.Add(new Skills { Name = "Religion", Stat = "Intelligence", Prof = "" });
                skills.Add(new Skills { Name = "Animal Handling", Stat = "Wisdom", Prof = "" });
                skills.Add(new Skills { Name = "Insight", Stat = "Wisdom", Prof = "" });
                skills.Add(new Skills { Name = "Medicine", Stat = "Wisdom", Prof = "" });
                skills.Add(new Skills { Name = "Perception", Stat = "Wisdom", Prof = "" });
                skills.Add(new Skills { Name = "Survival", Stat = "Wisdom", Prof = "" });
                skills.Add(new Skills { Name = "Deception", Stat = "Charisma", Prof = "" });
                skills.Add(new Skills { Name = "Intimidation", Stat = "Charisma", Prof = "" });
                skills.Add(new Skills { Name = "Performance", Stat = "Charisma", Prof = "" });
                skills.Add(new Skills { Name = "Persuasion", Stat = "Charisma", Prof = "" });


            }

            if (features != null)
            {
                features.Add("Darkvision", "Accustomed to life underground, you have superior vision in dark and dim conditions. You can see in dim light within 60 feet of you as if it were bright light, and in darkness as if it were dim light. You can't discern color in darkness, only shades of gray.");
                features.Add("Size", "Goblins are between 3 and 4 feet tall and weigh between 40 and 80 pounds. Your size is Small.");
            }
            Name = "NPC";
            lvl = 10;

            Hp_hpmax[0] = 10;
            Hp_hpmax[1] = 10;

        }
    }
}
