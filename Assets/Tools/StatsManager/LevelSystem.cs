using System;

[Serializable]
public class LevelSystem
{
    public class ExpIncreaseResponse
    {
        public int numberOfLevelsGained;
        public int currentLevel;
        public float normalizedExp;
    }

    public event Action<ExpIncreaseResponse> OnLevelUp;

    private CustomAnimationCurve levelCurve;
    private int currentLevel;
    private float currentExp;

    public CustomAnimationCurve LevelCurve => levelCurve;
    public int CurrentLevel => currentLevel;
    public float CurrentExp => currentExp;

    public LevelSystem(int currenLevel, float currentExp, CustomAnimationCurve levelCurve)
    {
        this.currentLevel = currenLevel;
        this.currentExp = currentExp;
        this.levelCurve = levelCurve;
    }

    public int TotalExpAccumulated(int currentLevel) => levelCurve.GetTotalValue(currentLevel);
    public float GetNormalizedExp() => currentExp / GetExpForNextLevel(currentLevel);
    public int GetExpForLevel(int level) => levelCurve.GetCurrentValueInt(level);
    public float GetExpForNextLevel(int currentLevel) => levelCurve.GetCurrentValueInt(currentLevel + 1);


    public Tuple<int, float> GetNumberOfLevelsGained(float extraExp)
    {
        int numberOfLevelsGained = 0;
        float exp = currentExp + extraExp;
        while (exp >= GetExpForNextLevel(currentLevel))
        {
            exp -= GetExpForNextLevel(currentLevel);
            ++numberOfLevelsGained;
        }
        float normalizedExp = exp / GetExpForNextLevel(currentLevel + numberOfLevelsGained);
        return new Tuple<int, float>(numberOfLevelsGained, normalizedExp);
    }

    public void AddExp(float exp)
    {
        SetExp(currentExp + exp);
    }

    public void SetExp(float exp) 
    {
        currentExp = exp;
        int numberOfLevelsGained = 0;
        while (currentExp >= GetExpForNextLevel(currentLevel))
        {
            currentExp -= GetExpForNextLevel(currentLevel);
            currentLevel++;
            ++numberOfLevelsGained;
        }

        OnLevelUp?.Invoke(new ExpIncreaseResponse
        {
            numberOfLevelsGained = numberOfLevelsGained,
            currentLevel = currentLevel,
            normalizedExp = GetNormalizedExp()
        });
    }
}