using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

class Voxel
{
    public byte x;
    public byte y;
    public byte z;
    public byte colorIndex;

    public Voxel(byte x, byte y, byte z, byte colorIndex)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.colorIndex = colorIndex;
    }
}

static class VoxelReader
{
    public static List<Voxel> Read(string file)
    {
        List<Voxel> voxels = new List<Voxel>();

        using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
        using (BinaryReader reader = new BinaryReader(fs))
        {
            // Read the header
            string magic = new string(reader.ReadChars(4));
            if (magic != "VOX ")
            {
                throw new Exception("Not a valid VOX file");
            }

            int version = reader.ReadInt32();

            // Read the main chunk
            string mainChunkId = new string(reader.ReadChars(4));
            if (mainChunkId != "MAIN")
            {
                throw new Exception("Invalid VOX file: MAIN chunk not found");
            }

            int mainChunkSize = reader.ReadInt32();
            int mainChunkChildrenSize = reader.ReadInt32();

            while (reader.BaseStream.Position < mainChunkSize + mainChunkChildrenSize + 12)
            {
                string chunkId = new string(reader.ReadChars(4));
                int chunkSize = reader.ReadInt32();
                int childChunkSize = reader.ReadInt32();

                if (chunkId == "SIZE")
                {
                    // Skip SIZE chunk (12 bytes)
                    reader.BaseStream.Seek(chunkSize + childChunkSize, SeekOrigin.Current);
                }
                else if (chunkId == "XYZI")
                {
                    // Read the voxel data
                    int numVoxels = reader.ReadInt32();
                    for (int i = 0; i < numVoxels; i++)
                    {
                        byte x = reader.ReadByte();
                        byte y = reader.ReadByte();
                        byte z = reader.ReadByte();
                        byte colorIndex = reader.ReadByte();
                        voxels.Add(new Voxel(x, y, z, colorIndex));
                    }
                    reader.BaseStream.Seek(childChunkSize, SeekOrigin.Current);
                }
                else
                {
                    // Skip other chunks
                    reader.BaseStream.Seek(chunkSize + childChunkSize, SeekOrigin.Current);
                }
            }
        }

        return voxels;
    }
}