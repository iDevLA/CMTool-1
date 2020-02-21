﻿using ConceptMatrix.Models;
using ConceptMatrix.Utility;
using ConceptMatrix.ViewModel;
using MahApps.Metro.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ConceptMatrix.Resx;
using System.Windows.Input;
using System.Windows.Media;
using GearTuple = System.Tuple<int, int, int>;
using WepTuple = System.Tuple<int, int, int, int>;
using System.Windows.Data;

namespace ConceptMatrix.Views
{
    /// <summary>
    /// Interaction logic for EquipmentFlyOut.xaml
    /// </summary>
    public partial class EquipmentFlyOut : Flyout
    {
        public ExdCsvReader.Item[] _items;
        public static GearSet _cGearSet = new GearSet();
        private GearSet _gearSet = new GearSet();
        private ExdCsvReader.Resident[] _residents;
        public ExdCsvReader.Resident Choice = null;
        private bool isUserInteraction = false;
        public static bool UserDoneInteraction = false;
        public CharacterDetails CharacterDetails { get => (CharacterDetails)BaseViewModel.model; set => BaseViewModel.model = value; }

        public EquipmentFlyOut()
        {
            InitializeComponent();
            CheckIncluded.Visibility = Visibility.Hidden;
            KeepDyes.Visibility = Visibility.Hidden;
            CurrentlyEquippedName.Visibility = Visibility.Hidden;
            EquippedLabel.Visibility = Visibility.Hidden;
            ClassBox.Visibility = Visibility.Hidden;
        }

        private static ImageSource CreateSource(SaintCoinach.Imaging.ImageFile file)
        {
            if (file == null)
            {
                var bitmap = new System.Drawing.Bitmap((System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("Corrupted"));
                IntPtr bmpPt = bitmap.GetHbitmap();
                System.Windows.Media.Imaging.BitmapSource bitmapSource =
                 System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                       bmpPt,
                       IntPtr.Zero,
                       Int32Rect.Empty,
                       System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                //freeze bitmapSource and clear memory to avoid memory leaks
                bitmapSource.Freeze();
                SpecialControl.DeleteObject(bmpPt);
                return bitmapSource;
            }
                   
            var argb = SaintCoinach.Imaging.ImageConverter.GetA8R8G8B8(file);
            return System.Windows.Media.Imaging.BitmapSource.Create(
                                       file.Width, file.Height,
                96, 96,
                PixelFormats.Bgra32, null,
                argb, file.Width * 4);
        }
        public class Itemx
        {
            public int Index { get; set; }
            public string Name { get; set; }
            public string ModelMain { get; set; }
            public string ModelOff { get; set; }
            public ExdCsvReader.ItemType Type { get; set; }
            public ImageSource Icon { get; set; }
        }
        public void GearPicker(ExdCsvReader.Item[] items)
        {
            EquipBox.Items.Clear();
            _items = items;
            CurrentlyEquippedName.Content = "None";
            bool found = false;
            if (EquipBoxC.SelectedIndex > 11)
            {
                foreach (ExdCsvReader.Item game in _items)
                {
                    EquipBox.Items.Add(new Itemx
                    {
                        Name = game.Name,
                        ModelMain = game.ModelMain,
                        ModelOff = game.ModelOff,
                        Type = game.Type,
                        Icon = null
                    });
                }
            }
            else
            {
                string selectedTag = ((ComboBoxItem)ClassBox.SelectedItem).Tag.ToString();
                foreach (ExdCsvReader.Item game in _items)
                {
                    if(MainViewModel.RegionType=="Live" && SaveSettings.Default.Language == "zh" 
                        || MainViewModel.RegionType == "Live" && SaveSettings.Default.Language == "ko")
                    {
                        if (ClassBox.SelectedIndex != 0)
                        {
                            if (EquipBoxC.SelectedIndex != 0 && EquipBoxC.SelectedIndex != 1)
                            {
                                if (ClassBox.SelectedIndex == 4)
                                {
                                    if (!game.ClassJobListStringName.Equals(selectedTag)) continue;
                                }
                                else if (!game.ClassJobListStringName.Contains(selectedTag)) continue;
                            }
                            else if (ClassBox.SelectedIndex >= 7)
                            {
                                if (!game.ClassJobListStringName.Contains(selectedTag)) continue;
                            }
                        }
                    }
                    else if (ClassBox.SelectedIndex != 0)
                    {
                        if (EquipBoxC.SelectedIndex != 0 && EquipBoxC.SelectedIndex != 1)
                        {
                            if (ClassBox.SelectedIndex == 4)
                            {
                                if (!game.ClassJobListStringName.Equals(ClassBox.Text)) continue;
                            }
                            else if (!game.ClassJobListStringName.Contains(ClassBox.Text)) continue;
                        }
                        else if (ClassBox.SelectedIndex >= 7)
                        {
                            if (!game.ClassJobListStringName.Contains(ClassBox.Text)) continue;
                        }
                    }

                    EquipBox.Items.Add(new Itemx
                    {
                        Name = game.Name,
                        ModelMain = game.ModelMain,
                        ModelOff = game.ModelOff,
                        Type = game.Type,
                        Icon = CreateSource(game.Icon)
                    });
                    if (!found) // Only looking for a match once and will stop trying to match. 
                    {
                        var Modelmain = CommaToGearTuple(game.ModelMain);
                        if (EquipBoxC.SelectedIndex == 0)
                        {
                            if (CharacterDetails.Job.value > 0)
                            {
                                if (Modelmain.Item1 == CharacterDetails.Job.value && Modelmain.Item2 == CharacterDetails.WeaponBase.value && Modelmain.Item3 == CharacterDetails.WeaponV.value)
                                { CurrentlyEquippedName.Content = game.Name; found = true; }
                            }
                        }
                        else
                        if (EquipBoxC.SelectedIndex == 1)
                        {
                            var ModelOff = CommaToGearTuple(game.ModelOff);
                            if (CharacterDetails.Offhand.value > 0)
                            {
                                if (CharacterDetails.Job.value >= 200 && CharacterDetails.Job.value <= 299) // check if it's paladin sword
                                {
                                    if (Modelmain.Item1 == CharacterDetails.Offhand.value && Modelmain.Item2 == CharacterDetails.OffhandBase.value && Modelmain.Item3 == CharacterDetails.OffhandV.value)
                                    { CurrentlyEquippedName.Content = game.Name; found = true; }
                                }
                                else
                                {
                                    if (ModelOff.Item1 == CharacterDetails.Offhand.value && ModelOff.Item2 == CharacterDetails.OffhandBase.value && ModelOff.Item3 == CharacterDetails.OffhandV.value)
                                    { CurrentlyEquippedName.Content = game.Name; found = true; }
                                }
                            }
                        }
                        else
                        if (EquipBoxC.SelectedIndex == 2)
                        {
                            if (CharacterDetails.HeadPiece.value > 0)
                            {
                                if (Modelmain.Item1 == CharacterDetails.HeadPiece.value && Modelmain.Item2 == CharacterDetails.HeadV.value)
                                {
                                    CurrentlyEquippedName.Content = game.Name;
                                    found = true;
                                }
                            }
                        }
                        else
                        if (EquipBoxC.SelectedIndex == 3)
                        {
                            if (CharacterDetails.Chest.value > 0)
                            {
                                if (Modelmain.Item1 == CharacterDetails.Chest.value && Modelmain.Item2 == CharacterDetails.ChestV.value)
                                {
                                    CurrentlyEquippedName.Content = game.Name;
                                    found = true;
                                }
                            }
                        }
                        else
                        if (EquipBoxC.SelectedIndex == 4)
                        {
                            if (CharacterDetails.Arms.value > 0)
                            {
                                if (Modelmain.Item1 == CharacterDetails.Arms.value && Modelmain.Item2 == CharacterDetails.ArmsV.value)
                                {
                                    CurrentlyEquippedName.Content = game.Name;
                                    found = true;
                                }
                            }
                        }
                        else
                        if (EquipBoxC.SelectedIndex == 5)
                        {
                            if (CharacterDetails.Legs.value > 0)
                            {
                                if (Modelmain.Item1 == CharacterDetails.Legs.value && Modelmain.Item2 == CharacterDetails.LegsV.value)
                                {
                                    CurrentlyEquippedName.Content = game.Name;
                                    found = true;
                                }
                            }
                        }
                        else
                        if (EquipBoxC.SelectedIndex == 6)
                        {
                            if (CharacterDetails.Feet.value > 0)
                            {
                                if (Modelmain.Item1 == CharacterDetails.Feet.value && Modelmain.Item2 == CharacterDetails.FeetVa.value)
                                {
                                    CurrentlyEquippedName.Content = game.Name;
                                    found = true;
                                }
                            }
                        }
                        else
                        if (EquipBoxC.SelectedIndex == 7)
                        {
                            if (CharacterDetails.Ear.value > 0)
                            {
                                if (Modelmain.Item1 == CharacterDetails.Ear.value && Modelmain.Item2 == CharacterDetails.EarVa.value)
                                {
                                    CurrentlyEquippedName.Content = game.Name;
                                    found = true;
                                }
                            }
                        }
                        else
                        if (EquipBoxC.SelectedIndex == 8)
                        {
                            if (CharacterDetails.Neck.value > 0)
                            {
                                if (Modelmain.Item1 == CharacterDetails.Neck.value && Modelmain.Item2 == CharacterDetails.NeckVa.value)
                                {
                                    CurrentlyEquippedName.Content = game.Name;
                                    found = true;
                                }
                            }
                        }
                        else
                        if (EquipBoxC.SelectedIndex == 9)
                        {
                            if (CharacterDetails.Wrist.value > 0)
                            {
                                if (Modelmain.Item1 == CharacterDetails.Wrist.value && Modelmain.Item2 == CharacterDetails.WristVa.value)
                                {
                                    CurrentlyEquippedName.Content = game.Name;
                                    found = true;
                                }
                            }
                        }
                        else
                        if (EquipBoxC.SelectedIndex == 10)
                        {
                            if (CharacterDetails.RFinger.value > 0)
                            {
                                if (Modelmain.Item1 == CharacterDetails.RFinger.value && Modelmain.Item2 == CharacterDetails.RFingerVa.value)
                                {
                                    CurrentlyEquippedName.Content = game.Name;
                                    found = true;
                                }
                            }
                        }
                        else
                        if (EquipBoxC.SelectedIndex == 11)
                        {
                            if (CharacterDetails.LFinger.value > 0)
                            {
                                if (Modelmain.Item1 == CharacterDetails.LFinger.value && Modelmain.Item2 == CharacterDetails.LFingerVa.value)
                                {
                                    CurrentlyEquippedName.Content = game.Name;
                                    found = true;
                                }
                            }
                        }
                        else
                        { CurrentlyEquippedName.Content = "Unknown!? Unable to verifiy"; found = true; }
                    }
                }
            }
            if(CurrentlyEquippedName.Content.ToString()=="None")
            {
                if (EquipBoxC.SelectedIndex == 0 && CharacterDetails.Job.value > 0 ||
                    EquipBoxC.SelectedIndex == 1 && CharacterDetails.Offhand.value > 0 ||
                    EquipBoxC.SelectedIndex == 2 && CharacterDetails.HeadPiece.value > 0 ||
                    EquipBoxC.SelectedIndex == 3 && CharacterDetails.Chest.value > 0 ||
                    EquipBoxC.SelectedIndex == 4 && CharacterDetails.Arms.value > 0 ||
                    EquipBoxC.SelectedIndex == 5 && CharacterDetails.Legs.value > 0 ||
                    EquipBoxC.SelectedIndex == 6 && CharacterDetails.Feet.value > 0 ||
                    EquipBoxC.SelectedIndex == 7 && CharacterDetails.Ear.value > 0 ||
                    EquipBoxC.SelectedIndex == 8 && CharacterDetails.Neck.value > 0 ||
                    EquipBoxC.SelectedIndex == 9 && CharacterDetails.Wrist.value > 0 ||
                    EquipBoxC.SelectedIndex == 10 && CharacterDetails.RFinger.value > 0 ||
                    EquipBoxC.SelectedIndex == 11 && CharacterDetails.LFinger.value > 0) CurrentlyEquippedName.Content = "Unknown? Unable to verifiy";
            }
        }
        public static byte[] WepTupleToByteAry(WepTuple tuple)
        {
            byte[] bytes = new byte[8];

            BitConverter.GetBytes((Int16)tuple.Item1).CopyTo(bytes, 0);
            BitConverter.GetBytes((Int16)tuple.Item2).CopyTo(bytes, 2);
            BitConverter.GetBytes((Int16)tuple.Item3).CopyTo(bytes, 4);
            BitConverter.GetBytes((Int16)tuple.Item4).CopyTo(bytes, 6);

            return bytes;
        }
        public static GearTuple ReadGearTuple(string offset)
        {
            var bytes = MemoryManager.Instance.MemLib.readBytes(offset.ToString(), 4);

            return new GearTuple(BitConverter.ToInt16(bytes, 0), bytes[2], bytes[3]);
        }
        public static WepTuple ReadWepTuple(string offset)
        {
            var bytes = MemoryManager.Instance.MemLib.readBytes(offset, 8);

            return new WepTuple(BitConverter.ToInt16(bytes, 0), BitConverter.ToInt16(bytes, 2), BitConverter.ToInt16(bytes, 4), BitConverter.ToInt16(bytes, 6));
        }

        public static string WepTupleToComma(WepTuple tuple)
        {
            return $"{tuple.Item1},{tuple.Item2},{tuple.Item3},{tuple.Item4}";
        }
        public static WepTuple CommaToWepTuple(string input)
        {
            var parts = input.Split(',');
            return new WepTuple(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]));
        }
        public static string GearTupleToComma(GearTuple tuple)
        {
            return $"{tuple.Item1},{tuple.Item2},{tuple.Item3}";
        }

        public static GearTuple CommaToGearTuple(string input)
        {
            var parts = input.Split(',');
            return new GearTuple(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        }

        public static byte[] GearTupleToByteAry(GearTuple tuple)
        {
            byte[] bytes = new byte[4];

            BitConverter.GetBytes((Int16)tuple.Item1).CopyTo(bytes, 0);
            bytes[2] = (byte)tuple.Item2;
            bytes[3] = (byte)tuple.Item3;

            return bytes;
        }
        public void WriteGear_Click()
        {
            if (CharacterDetails.HeadPiece.freeze == true) { CharacterDetails.HeadPiece.freeze = false; CharacterDetails.HeadPiece.Activated = true; }
            if (CharacterDetails.Chest.freeze == true) { CharacterDetails.Chest.freeze = false; CharacterDetails.Chest.Activated = true; }
            if (CharacterDetails.Arms.freeze == true) { CharacterDetails.Arms.freeze = false; CharacterDetails.Arms.Activated = true; }
            if (CharacterDetails.Legs.freeze == true) { CharacterDetails.Legs.freeze = false; CharacterDetails.Legs.Activated = true; }
            if (CharacterDetails.Feet.freeze == true) { CharacterDetails.Feet.freeze = false; CharacterDetails.Feet.Activated = true; }
            if (CharacterDetails.Neck.freeze == true) { CharacterDetails.Neck.freeze = false; CharacterDetails.Neck.Activated = true; }
            if (CharacterDetails.Ear.freeze == true) { CharacterDetails.Ear.freeze = false; CharacterDetails.Ear.Activated = true; }
            if (CharacterDetails.Wrist.freeze == true) { CharacterDetails.Wrist.freeze = false; CharacterDetails.Wrist.Activated = true; }
            if (CharacterDetails.RFinger.freeze == true) { CharacterDetails.RFinger.freeze = false; CharacterDetails.RFinger.Activated = true; }
            if (CharacterDetails.LFinger.freeze == true) { CharacterDetails.LFinger.freeze = false; CharacterDetails.LFinger.Activated = true; }
            if (CharacterDetails.Job.freeze == true) { CharacterDetails.Job.freeze = false; CharacterDetails.Job.Activated = true; }
            if (CharacterDetails.Offhand.freeze == true) { CharacterDetails.Offhand.freeze = false; CharacterDetails.Offhand.Activated = true; }
            _cGearSet.HeadGear = CommaToGearTuple(CharacterDetails.HeadSlot.value);
            _cGearSet.BodyGear = CommaToGearTuple(CharacterDetails.BodySlot.value);
            _cGearSet.HandsGear = CommaToGearTuple(CharacterDetails.ArmSlot.value);
            _cGearSet.LegsGear = CommaToGearTuple(CharacterDetails.LegSlot.value);
            _cGearSet.FeetGear = CommaToGearTuple(CharacterDetails.FeetSlot.value);
            _cGearSet.NeckGear = CommaToGearTuple(CharacterDetails.NeckSlot.value);
            _cGearSet.EarGear = CommaToGearTuple(CharacterDetails.EarSlot.value);
            _cGearSet.RRingGear = CommaToGearTuple(CharacterDetails.RFingerSlot.value);
            _cGearSet.LRingGear = CommaToGearTuple(CharacterDetails.LFingerSlot.value);
            _cGearSet.WristGear = CommaToGearTuple(CharacterDetails.WristSlot.value);
            _cGearSet.MainWep = CommaToWepTuple(CharacterDetails.WeaponSlot.value);
            _cGearSet.OffWep = CommaToWepTuple(CharacterDetails.OffhandSlot.value);
            WriteCurrentGearTuples();
        }
        public void WriteCurrentGearTuples()
        {
            if (_cGearSet.HeadGear == null)
                return;
            CharacterDetails.Job.value = _cGearSet.MainWep.Item1;
            CharacterDetails.WeaponBase.value = _cGearSet.MainWep.Item2;
            CharacterDetails.WeaponV.value = (byte)_cGearSet.MainWep.Item3;
            if (KeepDyes.IsChecked == false)
            {
                CharacterDetails.WeaponDye.value = (byte)_cGearSet.MainWep.Item4;
            }
            else _cGearSet.MainWep = new WepTuple(_cGearSet.MainWep.Item1, _cGearSet.MainWep.Item2, _cGearSet.MainWep.Item3, CharacterDetails.WeaponDye.value);
            CharacterDetails.Offhand.value = _cGearSet.OffWep.Item1;
            CharacterDetails.OffhandBase.value = _cGearSet.OffWep.Item2;
            CharacterDetails.OffhandV.value = (byte)_cGearSet.OffWep.Item3;
            if (KeepDyes.IsChecked == false) CharacterDetails.OffhandDye.value = (byte)_cGearSet.OffWep.Item4;
            else _cGearSet.OffWep = new WepTuple(_cGearSet.OffWep.Item1, _cGearSet.OffWep.Item2, _cGearSet.OffWep.Item3, CharacterDetails.OffhandDye.value);
            CharacterDetails.HeadPiece.value = _cGearSet.HeadGear.Item1;
            CharacterDetails.HeadV.value = (byte)_cGearSet.HeadGear.Item2;
            if (KeepDyes.IsChecked == false) CharacterDetails.HeadDye.value = (byte)_cGearSet.HeadGear.Item3;
            else _cGearSet.HeadGear = new GearTuple(_cGearSet.HeadGear.Item1, _cGearSet.HeadGear.Item2, CharacterDetails.HeadDye.value);
            CharacterDetails.Chest.value = _cGearSet.BodyGear.Item1;
            CharacterDetails.ChestV.value = (byte)_cGearSet.BodyGear.Item2;
            if (KeepDyes.IsChecked == false) CharacterDetails.ChestDye.value = (byte)_cGearSet.BodyGear.Item3;
            else _cGearSet.BodyGear = new GearTuple(_cGearSet.BodyGear.Item1, _cGearSet.BodyGear.Item2, CharacterDetails.ChestDye.value);
            CharacterDetails.Arms.value = _cGearSet.HandsGear.Item1;
            CharacterDetails.ArmsV.value = (byte)_cGearSet.HandsGear.Item2;
            if (KeepDyes.IsChecked == false) CharacterDetails.ArmsDye.value = (byte)_cGearSet.HandsGear.Item3;
            else _cGearSet.HandsGear = new GearTuple(_cGearSet.HandsGear.Item1, _cGearSet.HandsGear.Item2, CharacterDetails.ArmsDye.value);
            CharacterDetails.Legs.value = _cGearSet.LegsGear.Item1;
            CharacterDetails.LegsV.value = (byte)_cGearSet.LegsGear.Item2;
            if (KeepDyes.IsChecked == false) CharacterDetails.LegsDye.value = (byte)_cGearSet.LegsGear.Item3;
            else _cGearSet.LegsGear = new GearTuple(_cGearSet.LegsGear.Item1, _cGearSet.LegsGear.Item2, CharacterDetails.LegsDye.value);
            CharacterDetails.Feet.value = _cGearSet.FeetGear.Item1;
            CharacterDetails.FeetVa.value = (byte)_cGearSet.FeetGear.Item2;
            if (KeepDyes.IsChecked == false) CharacterDetails.FeetDye.value = (byte)_cGearSet.FeetGear.Item3;
            else _cGearSet.FeetGear = new GearTuple(_cGearSet.FeetGear.Item1, _cGearSet.FeetGear.Item2, CharacterDetails.FeetDye.value);
            CharacterDetails.Neck.value = _cGearSet.NeckGear.Item1;
            CharacterDetails.NeckVa.value = (byte)_cGearSet.NeckGear.Item2;
            CharacterDetails.Ear.value = _cGearSet.EarGear.Item1;
            CharacterDetails.EarVa.value = (byte)_cGearSet.EarGear.Item2;
            CharacterDetails.Wrist.value = _cGearSet.WristGear.Item1;
            CharacterDetails.WristVa.value = (byte)_cGearSet.WristGear.Item2;
            CharacterDetails.RFinger.value = _cGearSet.RRingGear.Item1;
            CharacterDetails.RFingerVa.value = (byte)_cGearSet.RRingGear.Item2;
            CharacterDetails.LFinger.value = _cGearSet.LRingGear.Item1;
            CharacterDetails.LFingerVa.value = (byte)_cGearSet.LRingGear.Item2;
            MemoryManager.Instance.MemLib.writeBytes(MemoryManager.GetAddressString(CharacterDetailsViewModel.baseAddr, Settings.Instance.Character.HeadPiece), GearTupleToByteAry(_cGearSet.HeadGear));
            MemoryManager.Instance.MemLib.writeBytes(MemoryManager.GetAddressString(CharacterDetailsViewModel.baseAddr, Settings.Instance.Character.Chest), GearTupleToByteAry(_cGearSet.BodyGear));
            MemoryManager.Instance.MemLib.writeBytes(MemoryManager.GetAddressString(CharacterDetailsViewModel.baseAddr, Settings.Instance.Character.Arms), GearTupleToByteAry(_cGearSet.HandsGear));
            MemoryManager.Instance.MemLib.writeBytes(MemoryManager.GetAddressString(CharacterDetailsViewModel.baseAddr, Settings.Instance.Character.Legs), GearTupleToByteAry(_cGearSet.LegsGear));
            MemoryManager.Instance.MemLib.writeBytes(MemoryManager.GetAddressString(CharacterDetailsViewModel.baseAddr, Settings.Instance.Character.Feet), GearTupleToByteAry(_cGearSet.FeetGear));
            MemoryManager.Instance.MemLib.writeBytes(MemoryManager.GetAddressString(CharacterDetailsViewModel.baseAddr, Settings.Instance.Character.Job), WepTupleToByteAry(_cGearSet.MainWep));
            MemoryManager.Instance.MemLib.writeBytes(MemoryManager.GetAddressString(CharacterDetailsViewModel.baseAddr, Settings.Instance.Character.Offhand), WepTupleToByteAry(_cGearSet.OffWep));
            MemoryManager.Instance.MemLib.writeBytes(MemoryManager.GetAddressString(CharacterDetailsViewModel.baseAddr, Settings.Instance.Character.Ear), GearTupleToByteAry(_cGearSet.EarGear));
            MemoryManager.Instance.MemLib.writeBytes(MemoryManager.GetAddressString(CharacterDetailsViewModel.baseAddr, Settings.Instance.Character.Neck), GearTupleToByteAry(_cGearSet.NeckGear));
            MemoryManager.Instance.MemLib.writeBytes(MemoryManager.GetAddressString(CharacterDetailsViewModel.baseAddr, Settings.Instance.Character.Wrist), GearTupleToByteAry(_cGearSet.WristGear));
            MemoryManager.Instance.MemLib.writeBytes(MemoryManager.GetAddressString(CharacterDetailsViewModel.baseAddr, Settings.Instance.Character.RFinger), GearTupleToByteAry(_cGearSet.RRingGear));
            MemoryManager.Instance.MemLib.writeBytes(MemoryManager.GetAddressString(CharacterDetailsViewModel.baseAddr, Settings.Instance.Character.LFinger), GearTupleToByteAry(_cGearSet.LRingGear));
            if (CharacterDetails.HeadPiece.Activated == true) { CharacterDetails.HeadPiece.freeze = true; CharacterDetails.HeadPiece.Activated = false; }
            if (CharacterDetails.Chest.Activated == true) { CharacterDetails.Chest.freeze = true; CharacterDetails.Chest.Activated = false; }
            if (CharacterDetails.Arms.Activated == true) { CharacterDetails.Arms.freeze = true; CharacterDetails.Arms.Activated = false; }
            if (CharacterDetails.Legs.Activated == true) { CharacterDetails.Legs.freeze = true; CharacterDetails.Legs.Activated = false; }
            if (CharacterDetails.Feet.Activated == true) { CharacterDetails.Feet.freeze = true; CharacterDetails.Feet.Activated = false; }
            if (CharacterDetails.Neck.Activated == true) { CharacterDetails.Neck.freeze = true; CharacterDetails.Neck.Activated = false; }
            if (CharacterDetails.Ear.Activated == true) { CharacterDetails.Ear.freeze = true; CharacterDetails.Ear.Activated = false; }
            if (CharacterDetails.Wrist.Activated == true) { CharacterDetails.Wrist.freeze = true; CharacterDetails.Wrist.Activated = false; }
            if (CharacterDetails.RFinger.Activated == true) { CharacterDetails.RFinger.freeze = true; CharacterDetails.RFinger.Activated = false; }
            if (CharacterDetails.LFinger.Activated == true) { CharacterDetails.LFinger.freeze = true; CharacterDetails.LFinger.Activated = false; }
            if (CharacterDetails.Job.Activated == true) { CharacterDetails.Job.freeze = true; CharacterDetails.Job.Activated = false; }
            if (CharacterDetails.Offhand.Activated == true) { CharacterDetails.Offhand.freeze = true; CharacterDetails.Offhand.Activated = false; }
        }

        private void EquipBoxC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckIncluded.IsChecked = false;
            KeepDyes.IsChecked = SaveSettings.Default.KeepDyes;
            ClassBox.SelectedIndex = SaveSettings.Default.ClassIndex;
            CheckIncluded.Visibility = Visibility.Hidden;
            KeepDyes.Visibility = Visibility.Hidden;
            CurrentlyEquippedName.Visibility = Visibility.Hidden;
            EquippedLabel.Visibility = Visibility.Hidden;
            if (isUserInteraction)
            {
                if (!CharacterDetailsView2.CheckItemList())
                    return;
                CurrentlyEquippedName.Visibility = Visibility.Visible;
                EquippedLabel.Visibility = Visibility.Visible;
                ClassBox.Visibility = Visibility.Visible;
                if (EquipBoxC.SelectedIndex == 0)
                {
                    CheckIncluded.Visibility = Visibility.Visible;
                    KeepDyes.Visibility = Visibility.Visible;
                    CheckIncluded.Content = FlyOutStrings.IncludeOffhand;
                    GearPicker(CharacterDetailsView._exdProvider.Items.Values.Where(c => c.Type == ExdCsvReader.ItemType.Wep || c.Type == ExdCsvReader.ItemType.Shield).ToArray());
                }
                if (EquipBoxC.SelectedIndex == 1)
                {
                    CheckIncluded.Visibility = Visibility.Visible;
                    KeepDyes.Visibility = Visibility.Visible;
                    CheckIncluded.Content = FlyOutStrings.NoneOffHand;
                    GearPicker(CharacterDetailsView._exdProvider.Items.Values.Where(c => c.Type == ExdCsvReader.ItemType.Wep || c.Type == ExdCsvReader.ItemType.Shield).ToArray());
                }
                if (EquipBoxC.SelectedIndex == 2)
                {
                    KeepDyes.Visibility = Visibility.Visible;
                    GearPicker(CharacterDetailsView._exdProvider.Items.Values.Where(c => c.Type == ExdCsvReader.ItemType.Head).ToArray());
                }
                if (EquipBoxC.SelectedIndex == 3)
                {
                    KeepDyes.Visibility = Visibility.Visible;
                    GearPicker(CharacterDetailsView._exdProvider.Items.Values.Where(c => c.Type == ExdCsvReader.ItemType.Body).ToArray());
                }

                if (EquipBoxC.SelectedIndex == 4)
                {
                    KeepDyes.Visibility = Visibility.Visible;
                    GearPicker(CharacterDetailsView._exdProvider.Items.Values.Where(c => c.Type == ExdCsvReader.ItemType.Hands).ToArray());
                }
                if (EquipBoxC.SelectedIndex == 5)
                {
                    KeepDyes.Visibility = Visibility.Visible;
                    GearPicker(CharacterDetailsView._exdProvider.Items.Values.Where(c => c.Type == ExdCsvReader.ItemType.Legs).ToArray());
                }
                if (EquipBoxC.SelectedIndex == 6)
                {
                    KeepDyes.Visibility = Visibility.Visible;
                    GearPicker(CharacterDetailsView._exdProvider.Items.Values.Where(c => c.Type == ExdCsvReader.ItemType.Feet).ToArray());
                }

                if (EquipBoxC.SelectedIndex == 7) GearPicker(CharacterDetailsView._exdProvider.Items.Values.Where(c => c.Type == ExdCsvReader.ItemType.Ears).ToArray());
                if (EquipBoxC.SelectedIndex == 8) GearPicker(CharacterDetailsView._exdProvider.Items.Values.Where(c => c.Type == ExdCsvReader.ItemType.Neck).ToArray());
                if (EquipBoxC.SelectedIndex == 9) GearPicker(CharacterDetailsView._exdProvider.Items.Values.Where(c => c.Type == ExdCsvReader.ItemType.Wrists).ToArray());
                if (EquipBoxC.SelectedIndex == 10) GearPicker(CharacterDetailsView._exdProvider.Items.Values.Where(c => c.Type == ExdCsvReader.ItemType.Ring).ToArray());
                if (EquipBoxC.SelectedIndex == 11) GearPicker(CharacterDetailsView._exdProvider.Items.Values.Where(c => c.Type == ExdCsvReader.ItemType.Ring).ToArray());
                if (EquipBoxC.SelectedIndex == 12)
                {
                    if (!CharacterDetailsView2.CheckPropList())
                        return;
                    GearPicker(CharacterDetailsView._exdProvider.ItemsProps.Values.ToArray());
                }
                if (EquipBoxC.SelectedIndex == 13)
                {
                    if (!CharacterDetailsView2.CheckPropList())
                        return;
                    GearPicker(CharacterDetailsView._exdProvider.ItemsProps.Values.ToArray());
                }
                isUserInteraction = false;
            }
        }
        private void EquipBoxC_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            isUserInteraction = true;
        }
        private void EquipBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EquipBox.SelectedCells.Count > 0)
            {
                if (EquipBox.SelectedItem == null)
                    return;
                var Value = (Itemx)EquipBox.SelectedItem;
                if (EquipBoxC.SelectedIndex == 0)
                {
                    CharacterDetails.WeaponSlot.value = Value.ModelMain;
                    if (CheckIncluded.IsChecked == true && Value.Type != ExdCsvReader.ItemType.Shield) CharacterDetails.OffhandSlot.value = Value.ModelOff;
                    WriteGear_Click();
                }
                if (EquipBoxC.SelectedIndex == 1)
                {
                    if (CheckIncluded.IsChecked == true) CharacterDetails.OffhandSlot.value = Value.ModelMain;
                    else CharacterDetails.OffhandSlot.value = Value.ModelOff;
                    WriteGear_Click();
                }
                if (EquipBoxC.SelectedIndex == 2)
                {
                    CharacterDetails.HeadSlot.value = Value.ModelMain;
                    WriteGear_Click();
                }
                if (EquipBoxC.SelectedIndex == 3)
                {
                    CharacterDetails.BodySlot.value = Value.ModelMain;
                    WriteGear_Click();
                }
                if (EquipBoxC.SelectedIndex == 4)
                {
                    CharacterDetails.ArmSlot.value = Value.ModelMain;
                    WriteGear_Click();
                }
                if (EquipBoxC.SelectedIndex == 5)
                {
                    CharacterDetails.LegSlot.value = Value.ModelMain;
                    WriteGear_Click();
                }
                if (EquipBoxC.SelectedIndex == 6)
                {
                    CharacterDetails.FeetSlot.value = Value.ModelMain;
                    WriteGear_Click();
                }
                if (EquipBoxC.SelectedIndex == 7)
                {
                    CharacterDetails.EarSlot.value = Value.ModelMain;
                    WriteGear_Click();
                }
                if (EquipBoxC.SelectedIndex == 8)
                {
                    CharacterDetails.NeckSlot.value = Value.ModelMain;
                    WriteGear_Click();
                }
                if (EquipBoxC.SelectedIndex == 9)
                {
                    CharacterDetails.WristSlot.value = Value.ModelMain;
                    WriteGear_Click();
                }
                if (EquipBoxC.SelectedIndex == 10)
                {
                    CharacterDetails.RFingerSlot.value = Value.ModelMain;
                    WriteGear_Click();
                }
                if (EquipBoxC.SelectedIndex == 11)
                {
                    CharacterDetails.LFingerSlot.value = Value.ModelMain;
                    WriteGear_Click();
                }
                if (EquipBoxC.SelectedIndex == 12)
                {
                    CharacterDetails.WeaponSlot.value = Value.ModelMain;
                    WriteGear_Click();
                }
                if (EquipBoxC.SelectedIndex == 13)
                {
                    CharacterDetails.OffhandSlot.value = Value.ModelMain;
                    WriteGear_Click();
                }
            }
        }
        public void ResidentSelector(ExdCsvReader.Resident[] residents)
        {
            UserDoneInteraction = true;
            CurrentlyEquippedName.Visibility = Visibility.Hidden;
            EquippedLabel.Visibility = Visibility.Hidden;
            residentlist.Items.Clear();
            _residents = residents;
            foreach (ExdCsvReader.Resident resident in _residents) residentlist.Items.Add(resident);
            _residents = residents;
        }

        private void FillCustoms2()
        {
            CharacterDetails.TestArray.value = MemoryManager.ByteArrayToString(_cGearSet.Customize);
        }
        private void WriteCurrentCustomize()
        {
            if (_cGearSet.Customize == null)
            {
                if (CharacterDetails.LimbalEyes.Activated == true) { CharacterDetails.LimbalEyes.freeze = true; CharacterDetails.LimbalEyes.Activated = false; }
                if (CharacterDetails.Highlights.Activated == true) { CharacterDetails.Highlights.freeze = true; CharacterDetails.Highlights.Activated = false; }
                if (CharacterDetails.Race.Activated == true) { CharacterDetails.Race.freeze = true; CharacterDetails.Race.Activated = false; }
                if (CharacterDetails.Gender.Activated == true) { CharacterDetails.Gender.freeze = true; CharacterDetails.Gender.Activated = false; }
                if (CharacterDetails.BodyType.Activated == true) { CharacterDetails.BodyType.freeze = true; CharacterDetails.BodyType.Activated = false; }
                if (CharacterDetails.RHeight.Activated == true) { CharacterDetails.RHeight.freeze = true; CharacterDetails.RHeight.Activated = false; }
                if (CharacterDetails.Clan.Activated == true) { CharacterDetails.Clan.freeze = true; CharacterDetails.Clan.Activated = false; }
                if (CharacterDetails.Head.Activated == true) { CharacterDetails.Head.freeze = true; CharacterDetails.Head.Activated = false; }
                if (CharacterDetails.Hair.Activated == true) { CharacterDetails.Hair.freeze = true; CharacterDetails.Hair.Activated = false; }
                if (CharacterDetails.HighlightTone.Activated == true) { CharacterDetails.HighlightTone.freeze = true; CharacterDetails.HighlightTone.Activated = false; }
                if (CharacterDetails.Skintone.Activated == true) { CharacterDetails.Skintone.freeze = true; CharacterDetails.Skintone.Activated = false; }
                if (CharacterDetails.RightEye.Activated == true) { CharacterDetails.RightEye.freeze = true; CharacterDetails.RightEye.Activated = false; }
                if (CharacterDetails.LeftEye.Activated == true) { CharacterDetails.LeftEye.freeze = true; CharacterDetails.LeftEye.Activated = false; }
                if (CharacterDetails.HairTone.Activated == true) { CharacterDetails.HairTone.freeze = true; CharacterDetails.HairTone.Activated = false; }
                if (CharacterDetails.FacePaint.Activated == true) { CharacterDetails.FacePaint.freeze = true; CharacterDetails.FacePaint.Activated = false; }
                if (CharacterDetails.FacePaintColor.Activated == true) { CharacterDetails.FacePaintColor.freeze = true; CharacterDetails.FacePaintColor.Activated = false; }
                if (CharacterDetails.EyeBrowType.Activated == true) { CharacterDetails.EyeBrowType.freeze = true; CharacterDetails.EyeBrowType.Activated = false; }
                if (CharacterDetails.Nose.Activated == true) { CharacterDetails.Nose.freeze = true; CharacterDetails.Nose.Activated = false; }
                if (CharacterDetails.Eye.Activated == true) { CharacterDetails.Eye.freeze = true; CharacterDetails.Eye.Activated = false; }
                if (CharacterDetails.Jaw.Activated == true) { CharacterDetails.Jaw.freeze = true; CharacterDetails.Jaw.Activated = false; }
                if (CharacterDetails.Lips.Activated == true) { CharacterDetails.Lips.freeze = true; CharacterDetails.Lips.Activated = false; }
                if (CharacterDetails.LipsTone.Activated == true) { CharacterDetails.LipsTone.freeze = true; CharacterDetails.LipsTone.Activated = false; }
                if (CharacterDetails.TailorMuscle.Activated == true) { CharacterDetails.TailorMuscle.freeze = true; CharacterDetails.TailorMuscle.Activated = false; }
                if (CharacterDetails.TailType.Activated == true) { CharacterDetails.TailType.freeze = true; CharacterDetails.TailType.Activated = false; }
                if (CharacterDetails.FacialFeatures.Activated == true) { CharacterDetails.FacialFeatures.freeze = true; CharacterDetails.FacialFeatures.Activated = false; }
                if (CharacterDetails.RBust.Activated == true) { CharacterDetails.RBust.freeze = true; CharacterDetails.RBust.Activated = false; }
                return;
            }
            byte[] CharacterBytes = _cGearSet.Customize;
            CharacterDetails.Race.value = CharacterBytes[0];
            CharacterDetails.Gender.value = CharacterBytes[1];
            CharacterDetails.BodyType.value = CharacterBytes[2];
            CharacterDetails.RHeight.value = CharacterBytes[3];
            CharacterDetails.Clan.value = CharacterBytes[4];
            CharacterDetails.Head.value = CharacterBytes[5];
            CharacterDetails.Hair.value = CharacterBytes[6];
            CharacterDetails.Highlights.value = CharacterBytes[7];
            CharacterDetails.Skintone.value = CharacterBytes[8];
            CharacterDetails.RightEye.value = CharacterBytes[9];
            CharacterDetails.HairTone.value = CharacterBytes[10];
            CharacterDetails.HighlightTone.value = CharacterBytes[11];
            CharacterDetails.FacialFeatures.value = CharacterBytes[12];
            CharacterDetails.LimbalEyes.value = CharacterBytes[13];
            CharacterDetails.EyeBrowType.value = CharacterBytes[14];
            CharacterDetails.LeftEye.value = CharacterBytes[15];
            CharacterDetails.Eye.value = CharacterBytes[16];
            CharacterDetails.Nose.value = CharacterBytes[17];
            CharacterDetails.Jaw.value = CharacterBytes[18];
            CharacterDetails.Lips.value = CharacterBytes[19];
            CharacterDetails.LipsTone.value = CharacterBytes[20];
            CharacterDetails.TailorMuscle.value = CharacterBytes[21];
            CharacterDetails.TailType.value = CharacterBytes[22];
            CharacterDetails.RBust.value = CharacterBytes[23];
            CharacterDetails.FacePaint.value = CharacterBytes[24];
            CharacterDetails.FacePaintColor.value = CharacterBytes[25];
            MemoryManager.Instance.MemLib.writeBytes(MemoryManager.GetAddressString(CharacterDetailsViewModel.baseAddr, Settings.Instance.Character.Race), CharacterBytes);
            CharacterDetails.ModelType.value = _cGearSet.ModelType;
            MemoryManager.Instance.MemLib.writeMemory(MemoryManager.GetAddressString(CharacterDetailsViewModel.baseAddr, Settings.Instance.Character.ModelType),"int", _cGearSet.ModelType.ToString());
            Task.Delay(25).Wait();
            if (CharacterDetails.LimbalEyes.Activated == true) { CharacterDetails.LimbalEyes.freeze = true; CharacterDetails.LimbalEyes.Activated = false; }
            if (CharacterDetails.Highlights.Activated == true) { CharacterDetails.Highlights.freeze = true; CharacterDetails.Highlights.Activated = false; }
            if (CharacterDetails.Race.Activated == true) { CharacterDetails.Race.freeze = true; CharacterDetails.Race.Activated = false; }
            if (CharacterDetails.Gender.Activated == true) { CharacterDetails.Gender.freeze = true; CharacterDetails.Gender.Activated = false; }
            if (CharacterDetails.BodyType.Activated == true) { CharacterDetails.BodyType.freeze = true; CharacterDetails.BodyType.Activated = false; }
            if (CharacterDetails.RHeight.Activated == true) { CharacterDetails.RHeight.freeze = true; CharacterDetails.RHeight.Activated = false; }
            if (CharacterDetails.Clan.Activated == true) { CharacterDetails.Clan.freeze = true; CharacterDetails.Clan.Activated = false; }
            if (CharacterDetails.Head.Activated == true) { CharacterDetails.Head.freeze = true; CharacterDetails.Head.Activated = false; }
            if (CharacterDetails.Hair.Activated == true) { CharacterDetails.Hair.freeze = true; CharacterDetails.Hair.Activated = false; }
            if (CharacterDetails.HighlightTone.Activated == true) { CharacterDetails.HighlightTone.freeze = true; CharacterDetails.HighlightTone.Activated = false; }
            if (CharacterDetails.Skintone.Activated == true) { CharacterDetails.Skintone.freeze = true; CharacterDetails.Skintone.Activated = false; }
            if (CharacterDetails.RightEye.Activated == true) { CharacterDetails.RightEye.freeze = true; CharacterDetails.RightEye.Activated = false; }
            if (CharacterDetails.LeftEye.Activated == true) { CharacterDetails.LeftEye.freeze = true; CharacterDetails.LeftEye.Activated = false; }
            if (CharacterDetails.HairTone.Activated == true) { CharacterDetails.HairTone.freeze = true; CharacterDetails.HairTone.Activated = false; }
            if (CharacterDetails.FacePaint.Activated == true) { CharacterDetails.FacePaint.freeze = true; CharacterDetails.FacePaint.Activated = false; }
            if (CharacterDetails.FacePaintColor.Activated == true) { CharacterDetails.FacePaintColor.freeze = true; CharacterDetails.FacePaintColor.Activated = false; }
            if (CharacterDetails.EyeBrowType.Activated == true) { CharacterDetails.EyeBrowType.freeze = true; CharacterDetails.EyeBrowType.Activated = false; }
            if (CharacterDetails.Nose.Activated == true) { CharacterDetails.Nose.freeze = true; CharacterDetails.Nose.Activated = false; }
            if (CharacterDetails.Eye.Activated == true) { CharacterDetails.Eye.freeze = true; CharacterDetails.Eye.Activated = false; }
            if (CharacterDetails.Jaw.Activated == true) { CharacterDetails.Jaw.freeze = true; CharacterDetails.Jaw.Activated = false; }
            if (CharacterDetails.Lips.Activated == true) { CharacterDetails.Lips.freeze = true; CharacterDetails.Lips.Activated = false; }
            if (CharacterDetails.LipsTone.Activated == true) { CharacterDetails.LipsTone.freeze = true; CharacterDetails.LipsTone.Activated = false; }
            if (CharacterDetails.TailorMuscle.Activated == true) { CharacterDetails.TailorMuscle.freeze = true; CharacterDetails.TailorMuscle.Activated = false; }
            if (CharacterDetails.TailType.Activated == true) { CharacterDetails.TailType.freeze = true; CharacterDetails.TailType.Activated = false; }
            if (CharacterDetails.FacialFeatures.Activated == true) { CharacterDetails.FacialFeatures.freeze = true; CharacterDetails.FacialFeatures.Activated = false; }
            if (CharacterDetails.RBust.Activated == true) { CharacterDetails.RBust.freeze = true; CharacterDetails.RBust.Activated = false; }
        }
        private void FillCustoms()
        {
            CharacterDetails.TestArray.value = MemoryManager.ByteArrayToString(_cGearSet.Customize);
            if (CharacterDetails.HeadPiece.freeze == true) { CharacterDetails.HeadPiece.freeze = false; CharacterDetails.HeadPiece.Activated = true; }
            if (CharacterDetails.Chest.freeze == true) { CharacterDetails.Chest.freeze = false; CharacterDetails.Chest.Activated = true; }
            if (CharacterDetails.Arms.freeze == true) { CharacterDetails.Arms.freeze = false; CharacterDetails.Arms.Activated = true; }
            if (CharacterDetails.Legs.freeze == true) { CharacterDetails.Legs.freeze = false; CharacterDetails.Legs.Activated = true; }
            if (CharacterDetails.Feet.freeze == true) { CharacterDetails.Feet.freeze = false; CharacterDetails.Feet.Activated = true; }
            if (CharacterDetails.Neck.freeze == true) { CharacterDetails.Neck.freeze = false; CharacterDetails.Neck.Activated = true; }
            if (CharacterDetails.Ear.freeze == true) { CharacterDetails.Ear.freeze = false; CharacterDetails.Ear.Activated = true; }
            if (CharacterDetails.Wrist.freeze == true) { CharacterDetails.Wrist.freeze = false; CharacterDetails.Wrist.Activated = true; }
            if (CharacterDetails.RFinger.freeze == true) { CharacterDetails.RFinger.freeze = false; CharacterDetails.RFinger.Activated = true; }
            if (CharacterDetails.LFinger.freeze == true) { CharacterDetails.LFinger.freeze = false; CharacterDetails.LFinger.Activated = true; }
            if (CharacterDetails.Job.freeze == true) { CharacterDetails.Job.freeze = false; CharacterDetails.Job.Activated = true; }
            if (CharacterDetails.Offhand.freeze == true) { CharacterDetails.Offhand.freeze = false; CharacterDetails.Offhand.Activated = true; }
            CharacterDetails.HeadSlot.value = GearTupleToComma(_cGearSet.HeadGear);
            CharacterDetails.BodySlot.value = GearTupleToComma(_cGearSet.BodyGear);
            CharacterDetails.ArmSlot.value = GearTupleToComma(_cGearSet.HandsGear);
            CharacterDetails.LegSlot.value = GearTupleToComma(_cGearSet.LegsGear);
            CharacterDetails.FeetSlot.value = GearTupleToComma(_cGearSet.FeetGear);
            CharacterDetails.EarSlot.value = GearTupleToComma(_cGearSet.EarGear);
            CharacterDetails.NeckSlot.value = GearTupleToComma(_cGearSet.NeckGear);
            CharacterDetails.RFingerSlot.value = GearTupleToComma(_cGearSet.RRingGear);
            CharacterDetails.LFingerSlot.value = GearTupleToComma(_cGearSet.LRingGear);
            CharacterDetails.WeaponSlot.value = WepTupleToComma(_cGearSet.MainWep);
            CharacterDetails.OffhandSlot.value = WepTupleToComma(_cGearSet.OffWep);
        }
        private void residentlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (residentlist.SelectedCells.Count > 0)
            {
                if (residentlist.SelectedItem == null)
                    return;
                Choice = residentlist.SelectedItem as ExdCsvReader.Resident;
                var gs = Choice.Gear;
                if(LoadType.SelectedIndex==0)
                {
                    _cGearSet.Customize = gs.Customize;
                    _cGearSet = gs;
                    if (CharacterDetails.LimbalEyes.freeze == true) { CharacterDetails.LimbalEyes.freeze = false; CharacterDetails.LimbalEyes.Activated = true; }
                    if (CharacterDetails.Highlights.freeze == true) { CharacterDetails.Highlights.freeze = false; CharacterDetails.Highlights.Activated = true; }
                    if (CharacterDetails.Race.freeze == true) { CharacterDetails.Race.freeze = false; CharacterDetails.Race.Activated = true; }
                    if (CharacterDetails.Gender.freeze == true) { CharacterDetails.Gender.freeze = false; CharacterDetails.Gender.Activated = true; }
                    if (CharacterDetails.BodyType.freeze == true) { CharacterDetails.BodyType.freeze = false; CharacterDetails.BodyType.Activated = true; }
                    if (CharacterDetails.RHeight.freeze == true) { CharacterDetails.RHeight.freeze = false; CharacterDetails.RHeight.Activated = true; }
                    if (CharacterDetails.Clan.freeze == true) { CharacterDetails.Clan.freeze = false; CharacterDetails.Clan.Activated = true; }
                    if (CharacterDetails.Head.freeze == true) { CharacterDetails.Head.freeze = false; CharacterDetails.Head.Activated = true; }
                    if (CharacterDetails.Hair.freeze == true) { CharacterDetails.Hair.freeze = false; CharacterDetails.Hair.Activated = true; }
                    if (CharacterDetails.HighlightTone.freeze == true) { CharacterDetails.HighlightTone.freeze = false; CharacterDetails.HighlightTone.Activated = true; }
                    if (CharacterDetails.Skintone.freeze == true) { CharacterDetails.Skintone.freeze = false; CharacterDetails.Skintone.Activated = true; }
                    if (CharacterDetails.RightEye.freeze == true) { CharacterDetails.RightEye.freeze = false; CharacterDetails.RightEye.Activated = true; }
                    if (CharacterDetails.LeftEye.freeze == true) { CharacterDetails.LeftEye.freeze = false; CharacterDetails.LeftEye.Activated = true; }
                    if (CharacterDetails.HairTone.freeze == true) { CharacterDetails.HairTone.freeze = false; CharacterDetails.HairTone.Activated = true; }
                    if (CharacterDetails.FacePaint.freeze == true) { CharacterDetails.FacePaint.freeze = false; CharacterDetails.FacePaint.Activated = true; }
                    if (CharacterDetails.FacePaintColor.freeze == true) { CharacterDetails.FacePaintColor.freeze = false; CharacterDetails.FacePaintColor.Activated = true; }
                    if (CharacterDetails.EyeBrowType.freeze == true) { CharacterDetails.EyeBrowType.freeze = false; CharacterDetails.EyeBrowType.Activated = true; }
                    if (CharacterDetails.Nose.freeze == true) { CharacterDetails.Nose.freeze = false; CharacterDetails.Nose.Activated = true; }
                    if (CharacterDetails.Eye.freeze == true) { CharacterDetails.Eye.freeze = false; CharacterDetails.Eye.Activated = true; }
                    if (CharacterDetails.Jaw.freeze == true) { CharacterDetails.Jaw.freeze = false; CharacterDetails.Jaw.Activated = true; }
                    if (CharacterDetails.Lips.freeze == true) { CharacterDetails.Lips.freeze = false; CharacterDetails.Lips.Activated = true; }
                    if (CharacterDetails.LipsTone.freeze == true) { CharacterDetails.LipsTone.freeze = false; CharacterDetails.LipsTone.Activated = true; }
                    if (CharacterDetails.TailorMuscle.freeze == true) { CharacterDetails.TailorMuscle.freeze = false; CharacterDetails.TailorMuscle.Activated = true; }
                    if (CharacterDetails.TailType.freeze == true) { CharacterDetails.TailType.freeze = false; CharacterDetails.TailType.Activated = true; }
                    if (CharacterDetails.FacialFeatures.freeze == true) { CharacterDetails.FacialFeatures.freeze = false; CharacterDetails.FacialFeatures.Activated = true; }
                    if (CharacterDetails.RBust.freeze == true) { CharacterDetails.RBust.freeze = false; CharacterDetails.RBust.Activated = true; }
                    FillCustoms();
                    WriteCurrentGearTuples();
                    WriteCurrentCustomize();
                }
                if(LoadType.SelectedIndex==1)
                {
                    _cGearSet.Customize = gs.Customize;
                    if (CharacterDetails.LimbalEyes.freeze == true) { CharacterDetails.LimbalEyes.freeze = false; CharacterDetails.LimbalEyes.Activated = true; }
                    if (CharacterDetails.Highlights.freeze == true) { CharacterDetails.Highlights.freeze = false; CharacterDetails.Highlights.Activated = true; }
                    if (CharacterDetails.Race.freeze == true) { CharacterDetails.Race.freeze = false; CharacterDetails.Race.Activated = true; }
                    if (CharacterDetails.Gender.freeze == true) { CharacterDetails.Gender.freeze = false; CharacterDetails.Gender.Activated = true; }
                    if (CharacterDetails.BodyType.freeze == true) { CharacterDetails.BodyType.freeze = false; CharacterDetails.BodyType.Activated = true; }
                    if (CharacterDetails.RHeight.freeze == true) { CharacterDetails.RHeight.freeze = false; CharacterDetails.RHeight.Activated = true; }
                    if (CharacterDetails.Clan.freeze == true) { CharacterDetails.Clan.freeze = false; CharacterDetails.Clan.Activated = true; }
                    if (CharacterDetails.Head.freeze == true) { CharacterDetails.Head.freeze = false; CharacterDetails.Head.Activated = true; }
                    if (CharacterDetails.Hair.freeze == true) { CharacterDetails.Hair.freeze = false; CharacterDetails.Hair.Activated = true; }
                    if (CharacterDetails.HighlightTone.freeze == true) { CharacterDetails.HighlightTone.freeze = false; CharacterDetails.HighlightTone.Activated = true; }
                    if (CharacterDetails.Skintone.freeze == true) { CharacterDetails.Skintone.freeze = false; CharacterDetails.Skintone.Activated = true; }
                    if (CharacterDetails.RightEye.freeze == true) { CharacterDetails.RightEye.freeze = false; CharacterDetails.RightEye.Activated = true; }
                    if (CharacterDetails.LeftEye.freeze == true) { CharacterDetails.LeftEye.freeze = false; CharacterDetails.LeftEye.Activated = true; }
                    if (CharacterDetails.HairTone.freeze == true) { CharacterDetails.HairTone.freeze = false; CharacterDetails.HairTone.Activated = true; }
                    if (CharacterDetails.FacePaint.freeze == true) { CharacterDetails.FacePaint.freeze = false; CharacterDetails.FacePaint.Activated = true; }
                    if (CharacterDetails.FacePaintColor.freeze == true) { CharacterDetails.FacePaintColor.freeze = false; CharacterDetails.FacePaintColor.Activated = true; }
                    if (CharacterDetails.EyeBrowType.freeze == true) { CharacterDetails.EyeBrowType.freeze = false; CharacterDetails.EyeBrowType.Activated = true; }
                    if (CharacterDetails.Nose.freeze == true) { CharacterDetails.Nose.freeze = false; CharacterDetails.Nose.Activated = true; }
                    if (CharacterDetails.Eye.freeze == true) { CharacterDetails.Eye.freeze = false; CharacterDetails.Eye.Activated = true; }
                    if (CharacterDetails.Jaw.freeze == true) { CharacterDetails.Jaw.freeze = false; CharacterDetails.Jaw.Activated = true; }
                    if (CharacterDetails.Lips.freeze == true) { CharacterDetails.Lips.freeze = false; CharacterDetails.Lips.Activated = true; }
                    if (CharacterDetails.LipsTone.freeze == true) { CharacterDetails.LipsTone.freeze = false; CharacterDetails.LipsTone.Activated = true; }
                    if (CharacterDetails.TailorMuscle.freeze == true) { CharacterDetails.TailorMuscle.freeze = false; CharacterDetails.TailorMuscle.Activated = true; }
                    if (CharacterDetails.TailType.freeze == true) { CharacterDetails.TailType.freeze = false; CharacterDetails.TailType.Activated = true; }
                    if (CharacterDetails.FacialFeatures.freeze == true) { CharacterDetails.FacialFeatures.freeze = false; CharacterDetails.FacialFeatures.Activated = true; }
                    if (CharacterDetails.RBust.freeze == true) { CharacterDetails.RBust.freeze = false; CharacterDetails.RBust.Activated = true; }
                    WriteCurrentCustomize();
                }
                if(LoadType.SelectedIndex==2)
                {
                    _cGearSet = gs;
                    FillCustoms();
                    WriteCurrentGearTuples();
                }
            }
        }

        private void SearchModelBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EquipBoxC.SelectedItem == null)
                return;
            string filter = SearchModelBox.Text.ToLower();
            EquipBox.Items.Clear();
            string selectedTag = ((ComboBoxItem)ClassBox.SelectedItem).Tag.ToString();
            foreach (ExdCsvReader.Item game in _items.Where(g => g.Name.ToLower().Contains(filter)))
            {
                if (EquipBoxC.SelectedIndex > 11)
                {
                    EquipBox.Items.Add(new Itemx
                    {
                        Name = game.Name,
                        ModelMain = game.ModelMain,
                        ModelOff = game.ModelOff,
                        Type = game.Type,
                        Icon = null
                    });
                }
                else
                {
                    if(MainViewModel.RegionType=="Live" && SaveSettings.Default.Language == "zh" 
                        || MainViewModel.RegionType == "Live" && SaveSettings.Default.Language == "ko")
                    {
                        if (ClassBox.SelectedIndex != 0)
                        {
                            if (EquipBoxC.SelectedIndex != 0 && EquipBoxC.SelectedIndex != 1)
                            {
                                if (ClassBox.SelectedIndex == 4)
                                {
                                    if (!game.ClassJobListStringName.Equals(selectedTag)) continue;
                                }
                                else if (!game.ClassJobListStringName.Contains(selectedTag)) continue;
                            }
                            else if (ClassBox.SelectedIndex >= 7)
                            {
                                if (!game.ClassJobListStringName.Contains(selectedTag)) continue;
                            }
                        }
                    }
                    else if (ClassBox.SelectedIndex != 0)
                    {
                        if (EquipBoxC.SelectedIndex != 0 && EquipBoxC.SelectedIndex != 1)
                        {
                            if (ClassBox.SelectedIndex == 4)
                            {
                                if (!game.ClassJobListStringName.Equals(ClassBox.Text)) continue;
                            }
                            else if (!game.ClassJobListStringName.Contains(ClassBox.Text)) continue;
                        }
                        else if (ClassBox.SelectedIndex >= 7)
                        {
                            if (!game.ClassJobListStringName.Contains(ClassBox.Text)) continue;
                        }
                    }

                    EquipBox.Items.Add(new Itemx
                    {
                        Name = game.Name.ToString(),
                        ModelMain = game.ModelMain,
                        ModelOff = game.ModelOff,
                        Type = game.Type,
                        Icon = CreateSource(game.Icon)
                    });
                }
            }
        }

        private void SearchModelBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = SearchModelBox2.Text.ToLower();
            residentlist.Items.Clear();
            foreach (ExdCsvReader.Resident resident in _residents.Where(g => g.Name.ToLower().Contains(filter))) residentlist.Items.Add(resident);
        }

        private void AnimatedTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource is TabControl)
            {
                if (NPCTab.IsSelected)
                {
                    if (!CharacterDetailsView2.CheckResidentList()) return;
                    if(!UserDoneInteraction)ResidentSelector(CharacterDetailsView._exdProvider.Residents.Values.Where(c => c.IsGoodNpc()).ToArray());
                    CurrentlyEquippedName.Visibility = Visibility.Hidden;
                    EquippedLabel.Visibility = Visibility.Hidden;
                    ClassBox.Visibility = Visibility.Hidden;
                }
                else
                {
                    CurrentlyEquippedName.Visibility = Visibility.Visible;
                    EquippedLabel.Visibility = Visibility.Visible;
                    ClassBox.Visibility = Visibility.Visible;
                }
            }
            else return;
            e.Handled = true;
        }

        private void KeepDyes_Checked(object sender, RoutedEventArgs e)
        {
            SaveSettings.Default.KeepDyes = true;
        }

        private void KeepDyes_Unchecked(object sender, RoutedEventArgs e)
        {
            SaveSettings.Default.KeepDyes = false;
        }

        private void ClassBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClassBox.IsKeyboardFocusWithin || ClassBox.IsMouseOver)
            {
                if (ClassBox.SelectedIndex < 0) return;
                SaveSettings.Default.ClassIndex = ClassBox.SelectedIndex;
            }
        }
    }
}