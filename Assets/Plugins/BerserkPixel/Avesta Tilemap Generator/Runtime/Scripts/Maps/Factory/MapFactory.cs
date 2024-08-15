using BerserkPixel.Tilemap_Generator.Algorithms;
using BerserkPixel.Tilemap_Generator.SO;

namespace BerserkPixel.Tilemap_Generator.Factory {
 public abstract class MapFactory {
    protected MapConfigSO mapConfig;

    protected MapFactory(MapConfigSO mapConfig) {
        this.mapConfig = mapConfig;
    }

    public abstract IMapAlgorithm CreateMap();
}

#region Concrete Factories

public class BasicRandomFactory : MapFactory {
    private IMapAlgorithm _cachedAlgorithm;

    public BasicRandomFactory(MapConfigSO mapConfig) : base(mapConfig) { }

    public override IMapAlgorithm CreateMap() {
        return _cachedAlgorithm ??= new BasicRandom(mapConfig);
    }
}

public class CellularAutomataFactory : MapFactory {
    private IMapAlgorithm _cachedAlgorithm;

    public CellularAutomataFactory(MapConfigSO mapConfig) : base(mapConfig) { }

    public override IMapAlgorithm CreateMap() {
        return _cachedAlgorithm ??= new CellularAutomata(mapConfig);
    }
}

public class DomainWarpFactory : MapFactory {
    private IMapAlgorithm _cachedAlgorithm;

    public DomainWarpFactory(MapConfigSO mapConfig) : base(mapConfig) { }

    public override IMapAlgorithm CreateMap() {
        return _cachedAlgorithm ??= new DomainWarping(mapConfig);
    }
}

public class FourierNoiseFactory : MapFactory {
    private IMapAlgorithm _cachedAlgorithm;

    public FourierNoiseFactory(MapConfigSO mapConfig) : base(mapConfig) { }

    public override IMapAlgorithm CreateMap() {
        return _cachedAlgorithm ??= new FourierNoise(mapConfig);
    }
}

public class FractalNoiseFactory : MapFactory {
    private IMapAlgorithm _cachedAlgorithm;

    public FractalNoiseFactory(MapConfigSO mapConfig) : base(mapConfig) { }

    public override IMapAlgorithm CreateMap() {
        return _cachedAlgorithm ??= new FractalNoise(mapConfig);
    }
}

public class FullGridFactory : MapFactory {
    private IMapAlgorithm _cachedAlgorithm;

    public FullGridFactory(MapConfigSO mapConfig) : base(mapConfig) { }

    public override IMapAlgorithm CreateMap() {
        return _cachedAlgorithm ??= new FullGrid(mapConfig);
    }
}

public class GameOfLifeFactory : MapFactory {
    private IMapAlgorithm _cachedAlgorithm;

    public GameOfLifeFactory(MapConfigSO mapConfig) : base(mapConfig) { }

    public override IMapAlgorithm CreateMap() {
        return _cachedAlgorithm ??= new GameOfLife(mapConfig);
    }
}

public class IslandFactory : MapFactory {
    private IMapAlgorithm _cachedAlgorithm;

    public IslandFactory(MapConfigSO mapConfig) : base(mapConfig) { }

    public override IMapAlgorithm CreateMap() {
        return _cachedAlgorithm ??= new Island(mapConfig);
    }
}

public class PathFactory : MapFactory {
    private IMapAlgorithm _cachedAlgorithm;

    public PathFactory(MapConfigSO mapConfig) : base(mapConfig) { }

    public override IMapAlgorithm CreateMap() {
        return _cachedAlgorithm ??= new Path(mapConfig);
    }
}

public class PerlinNoiseFactory : MapFactory {
    private IMapAlgorithm _cachedAlgorithm;

    public PerlinNoiseFactory(MapConfigSO mapConfig) : base(mapConfig) { }

    public override IMapAlgorithm CreateMap() {
        return _cachedAlgorithm ??= new PerlinNoise(mapConfig);
    }
}

public class RandomWalkFactory : MapFactory {
    private IMapAlgorithm _cachedAlgorithm;

    public RandomWalkFactory(MapConfigSO mapConfig) : base(mapConfig) { }

    public override IMapAlgorithm CreateMap() {
        return _cachedAlgorithm ??= new RandomWalk(mapConfig);
    }
}

public class RidgedMultifractalFactory : MapFactory {
    private IMapAlgorithm _cachedAlgorithm;

    public RidgedMultifractalFactory(MapConfigSO mapConfig) : base(mapConfig) { }

    public override IMapAlgorithm CreateMap() {
        return _cachedAlgorithm ??= new RidgedMultifractal(mapConfig);
    }
}

public class SparseConvolutionFactory : MapFactory {
    private IMapAlgorithm _cachedAlgorithm;

    public SparseConvolutionFactory(MapConfigSO mapConfig) : base(mapConfig) { }

    public override IMapAlgorithm CreateMap() {
        return _cachedAlgorithm ??= new SparseConvolution(mapConfig);
    }
}

public class VoronoiFactory : MapFactory {
    private IMapAlgorithm _cachedAlgorithm;

    public VoronoiFactory(MapConfigSO mapConfig) : base(mapConfig) { }

    public override IMapAlgorithm CreateMap() {
        return _cachedAlgorithm ??= new VoronoiNoise(mapConfig);
    }
}

public class WaveletNoiseFactory : MapFactory {
    private IMapAlgorithm _cachedAlgorithm;

    public WaveletNoiseFactory(MapConfigSO mapConfig) : base(mapConfig) { }

    public override IMapAlgorithm CreateMap() {
        return _cachedAlgorithm ??= new WaveletNoise(mapConfig);
    }
}

#endregion   
}