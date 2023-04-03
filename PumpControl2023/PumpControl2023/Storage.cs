using System;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Storage.Provider;
using GHIElectronics.TinyCLR.Devices.Storage;
using GHIElectronics.TinyCLR.Native;
using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Controls;
using GHIElectronics.TinyCLR.UI.Media;
using GHIElectronics.TinyCLR.IO.TinyFileSystem;
using System.IO;
using System.Diagnostics;

namespace PumpControl2023
{
    public class MyFileSystem
    {
        TinyFileSystem theFileSystem;

        public MyFileSystem()
        {
            const int CLUSTER_SIZE = 1024;

            theFileSystem = new TinyFileSystem(new QspiMemory(), CLUSTER_SIZE);

            if (!theFileSystem.CheckIfFormatted())
            {
                //Do Format if necessary 
                theFileSystem.Format();
            }
            else
            {
                // Mount tiny file system
                theFileSystem.Mount();
            }            
        }



        public void SaveToVialSlot(DispenseSetting s, int slot)
        {            
            if (slot < 1) return;
            if (slot > 4) return;
            string filename = "V" + slot.ToString() + ".dat}";
            using (var fsWrite = theFileSystem.Create(filename))
            {
                using (var wr = new StreamWriter(fsWrite))
                {
                    wr.WriteLine(s.ToString());
                    wr.Flush();
                    fsWrite.Flush();
                }
            }
        }

        public void SaveToBottleSlot(DispenseSetting s, int slot)
        {
            if (slot < 1) return;
            if (slot > 4) return;
            string filename = "B" + slot.ToString() + ".dat}";
            using (var fsWrite = theFileSystem.Create(filename))
            {
                using (var wr = new StreamWriter(fsWrite))
                {
                    wr.WriteLine(s.ToString());
                    wr.Flush();
                    fsWrite.Flush();
                }
            }
        }

        DispenseSetting ReadBottleSlot(int slot)
        {
            if (slot < 1) return null;
            if (slot > 4) return null;
            string filename = "B" + slot.ToString() + ".dat}";
            try { 
            using (var fsRead = theFileSystem.Open(filename, FileMode.Open))
            {
                using (var rdr = new StreamReader(fsRead))
                {
                    System.String line;
                    line = rdr.ReadLine();
                    return new DispenseSetting(line);
                }
            }
            }
            catch
            {
                return new DispenseSetting();
            }

        }

        DispenseSetting ReadVialSlot(int slot)
        {
            if (slot < 1) return null;
            if (slot > 4) return null;
            string filename = "B" + slot.ToString() + ".dat}";
            try
            {
                using (var fsRead = theFileSystem.Open(filename, FileMode.Open))
                {
                    using (var rdr = new StreamReader(fsRead))
                    {
                        System.String line;
                        line = rdr.ReadLine();
                        return new DispenseSetting(line);
                    }
                }
            }
            catch
            {
                return new DispenseSetting();
            }

        }
    
    }
}


    public sealed class QspiMemory : IStorageControllerProvider
    {
        
        public StorageDescriptor Descriptor => this.descriptor;
        const int SectorSize = 4 * 1024;

        private StorageDescriptor descriptor = new StorageDescriptor()
        {
            CanReadDirect = false,
            CanWriteDirect = false,
            CanExecuteDirect = false,
            EraseBeforeWrite = true,
            Removable = true,
            RegionsContiguous = true,
            RegionsEqualSized = true,
            RegionAddresses = new long[] { 0 },
            RegionSizes = new int[] { SectorSize },
            RegionCount = (2 * 1024 * 1024) / (SectorSize)
        };

        private IStorageControllerProvider qspiDrive;

        public QspiMemory() : this(2 * 1024 * 1024)
        {

        }

        public QspiMemory(uint size)
        {
            var maxSize = Flash.IsEnabledExtendDeployment ? (10 * 1024 * 1024) : (16 * 1024 * 1024);

            if (size > maxSize)
                throw new ArgumentOutOfRangeException("size too large.");

            if (size <= SectorSize)
                throw new ArgumentOutOfRangeException("size too small.");

            if (size != descriptor.RegionCount * SectorSize)
            {
                descriptor.RegionCount = (int)(size / SectorSize);
            }

            qspiDrive = StorageController.FromName(SC20260.StorageController.QuadSpi).Provider;

            this.Open();
        }

        public void Open()
        {
            qspiDrive.Open();
        }

        public void Close()
        {
            qspiDrive.Close();
        }

        public void Dispose()
        {
            qspiDrive.Dispose();
        }

        public int Erase(long address, int count, TimeSpan timeout)
        {
            return qspiDrive.Erase(address, count, timeout);
        }

        public bool IsErased(long address, int count)
        {
            return qspiDrive.IsErased(address, count);
        }

        public int Read(long address, int count, byte[] buffer, int offset, TimeSpan timeout)
        {
            return qspiDrive.Read(address, count, buffer, offset, timeout);
        }

        public int Write(long address, int count, byte[] buffer, int offset, TimeSpan timeout)
        {
            return qspiDrive.Write(address, count, buffer, offset, timeout);
        }

        public void EraseAll(TimeSpan timeout)
        {
            for (var sector = 0; sector < this.Descriptor.RegionCount; sector++)
            {
                qspiDrive.Erase(sector * this.Descriptor.RegionSizes[0], this.Descriptor.RegionSizes[0], timeout);
            }
        }
    }
