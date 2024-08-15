using System;

namespace BerserkPixel.Tilemap_Generator {
    [Serializable]
    public enum MapType {
        CellularAutomata = 0,
        PerlinNoise = 1,
        GameOfLife = 2,
        FullGrid = 3,
        BasicRandom = 4,
        RandomWalk = 5,
        Path = 6,
        Island = 7,
        FractalNoise = 8,
        DomainWarping = 9,
        RidgedMultifractal = 10,
        WaveletNoise = 11,
        FourierNoise = 12,
        Voronoi = 13,
        SparseConvolution = 14
    }
}