[System.Serializable]
public class GroundData
{
    public GroundPosition position;
    public float rotation;
    public GroundScale scale;
}

[System.Serializable]
public class GroundPosition
{
    public float x;
    public float z;

    public GroundPosition(float x, float z)
    {
        this.x = x;
        this.z = z;
    }
}

[System.Serializable]
public class GroundScale
{
    public float x;
    public float z;

    public GroundScale(float x, float z)
    {
        this.x = x;
        this.z = z;
    }
}