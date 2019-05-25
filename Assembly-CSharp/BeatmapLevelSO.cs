using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


// Token: 0x020001CD RID: 461
public class BeatmapLevelSO 
{
	// Token: 0x17000148 RID: 328
	// (get) Token: 0x0600076A RID: 1898 RVA: 0x0001AB8E File Offset: 0x00018D8E
	/*public string levelID
	{
		get
		{
			return this._levelID;
		}
	}

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x0600076B RID: 1899 RVA: 0x0001AB96 File Offset: 0x00018D96
	public string songName
	{
		get
		{
			return this._songName;
		}
	}

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x0600076C RID: 1900 RVA: 0x0001AB9E File Offset: 0x00018D9E
	public string songSubName
	{
		get
		{
			return this._songSubName;
		}
	}

	// Token: 0x1700014B RID: 331
	// (get) Token: 0x0600076D RID: 1901 RVA: 0x0001ABA6 File Offset: 0x00018DA6
	public string songAuthorName
	{
		get
		{
			return this._songAuthorName;
		}
	}

	// Token: 0x1700014C RID: 332
	// (get) Token: 0x0600076E RID: 1902 RVA: 0x0001ABAE File Offset: 0x00018DAE
	public string levelAuthorName
	{
		get
		{
			return this._levelAuthorName;
		}
	}

	// Token: 0x1700014D RID: 333
	// (get) Token: 0x0600076F RID: 1903 RVA: 0x0001ABB6 File Offset: 0x00018DB6
	public float beatsPerMinute
	{
		get
		{
			return this._beatsPerMinute;
		}
	}
    
	// Token: 0x1700014E RID: 334
	// (get) Token: 0x06000770 RID: 1904 RVA: 0x0001ABBE File Offset: 0x00018DBE
	public float songTimeOffset
	{
		get
		{
			return this._songTimeOffset;
		}
	}

	// Token: 0x1700014F RID: 335
	// (get) Token: 0x06000771 RID: 1905 RVA: 0x0001ABC6 File Offset: 0x00018DC6
	public float shuffle
	{
		get
		{
			return this._shuffle;
		}
	}

	// Token: 0x17000150 RID: 336
	// (get) Token: 0x06000772 RID: 1906 RVA: 0x0001ABCE File Offset: 0x00018DCE
	public float shufflePeriod
	{
		get
		{
			return this._shufflePeriod;
		}
	}
    
	// Token: 0x17000151 RID: 337
	// (get) Token: 0x06000773 RID: 1907 RVA: 0x0001ABD6 File Offset: 0x00018DD6
	//public AudioClip previewAudioClip
	//{
	//	get
	//	{
	//		return this._audioClip;
	//	}
	//}
//
	// Token: 0x17000152 RID: 338
	// (get) Token: 0x06000774 RID: 1908 RVA: 0x0001ABDE File Offset: 0x00018DDE
	public float previewStartTime
	{
		get
		{
			return this._previewStartTime;
		}
	}

	// Token: 0x17000153 RID: 339
	// (get) Token: 0x06000775 RID: 1909 RVA: 0x0001ABE6 File Offset: 0x00018DE6
	public float previewDuration
	{
		get
		{
			return this._previewDuration;
		}
	}
    */
	// Token: 0x17000154 RID: 340
	// (get) Token: 0x06000776 RID: 1910 RVA: 0x0001ABEE File Offset: 0x00018DEE
	//public Texture2D coverImageTexture2D
//	{
	//	get
	//	{
	//		return this._coverImageTexture2D;
	//	}
//	}
//
	// Token: 0x17000155 RID: 341
	// (get) Token: 0x06000777 RID: 1911 RVA: 0x0001ABF6 File Offset: 0x00018DF6
	/*public SceneInfo environmentSceneInfo
	{
		get
		{
			return this._environmentSceneInfo;
		}
	}

	// Token: 0x17000156 RID: 342
	// (get) Token: 0x06000778 RID: 1912 RVA: 0x0001AC00 File Offset: 0x00018E00
	public IDifficultyBeatmapSet[] difficultyBeatmapSets
	{
		get
		{
			return this._difficultyBeatmapSets;
		}
	}

	// Token: 0x17000157 RID: 343
	// (get) Token: 0x06000779 RID: 1913 RVA: 0x0001AC15 File Offset: 0x00018E15
	public float songDuration
	{
		get
		{
			return this._audioClip.length;
		}
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x0001AC24 File Offset: 0x00018E24
	public async Task<AudioClip> GetPreviewAudioClipAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		return await Task.FromResult<AudioClip>(this._audioClip);
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x0001AC74 File Offset: 0x00018E74
	public async Task<Texture2D> GetCoverImageTexture2DAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		return await Task.FromResult<Texture2D>(this._coverImageTexture2D);
	}

	// Token: 0x17000158 RID: 344
	// (get) Token: 0x0600077C RID: 1916 RVA: 0x0001ACC4 File Offset: 0x00018EC4
	public BeatmapLevelData beatmapLevelData
	{
		get
		{
			if (this._beatmapLevelData == null)
			{
				AudioClip audioClip = this._audioClip;
				IDifficultyBeatmapSet[] difficultyBeatmapSets = this._difficultyBeatmapSets;
				this._beatmapLevelData = new BeatmapLevelData(audioClip, difficultyBeatmapSets);
			}
			return this._beatmapLevelData;
		}
	}

	// Token: 0x17000159 RID: 345
	// (get) Token: 0x0600077D RID: 1917 RVA: 0x0001ACF8 File Offset: 0x00018EF8
	public BeatmapCharacteristicSO[] beatmapCharacteristics
	{
		get
		{
			return this._beatmapCharacteristics;
		}
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x0001AD00 File Offset: 0x00018F00
	public virtual async Task<GetBeatmapLevelDataResult> GetBeatmapLevelDataAsync(CancellationToken token)
	{
		if (this._beatmapLevelData == null)
		{
			AudioClip audioClip = this._audioClip;
			IDifficultyBeatmapSet[] difficultyBeatmapSets = this._difficultyBeatmapSets;
			this._beatmapLevelData = new BeatmapLevelData(audioClip, difficultyBeatmapSets);
			this._getBeatmapLevelDataResult = new GetBeatmapLevelDataResult(GetBeatmapLevelDataResult.Result.OK, this._beatmapLevelData);
		}
		return await Task.FromResult<GetBeatmapLevelDataResult>(this._getBeatmapLevelDataResult);
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x0001AD48 File Offset: 0x00018F48
	public virtual void InitFull(string levelID, string songName, string songSubName, string songAuthorName, string levelAuthorName, AudioClip audioClip, float beatsPerMinute, float songTimeOffset, float shuffle, float shufflePeriod, float previewStartTime, float previewDuration, Texture2D coverImageTexture2D, SceneInfo environmentSceneInfo, BeatmapLevelSO.DifficultyBeatmapSet[] difficultyBeatmapSets)
	{
		this._levelID = levelID;
		this._songName = songName;
		this._songSubName = songSubName;
		this._songAuthorName = songAuthorName;
		this._levelAuthorName = levelAuthorName;
		this._audioClip = audioClip;
		this._beatsPerMinute = beatsPerMinute;
		this._songTimeOffset = songTimeOffset;
		this._shuffle = shuffle;
		this._shufflePeriod = shufflePeriod;
		this._previewStartTime = previewStartTime;
		this._previewDuration = previewDuration;
		this._coverImageTexture2D = coverImageTexture2D;
		this._environmentSceneInfo = environmentSceneInfo;
		this._difficultyBeatmapSets = difficultyBeatmapSets;
		this.InitData();
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x0001ADD0 File Offset: 0x00018FD0
	public virtual void InitData()
	{
		AudioClip audioClip = this._audioClip;
		IDifficultyBeatmapSet[] difficultyBeatmapSets = this._difficultyBeatmapSets;
		this._beatmapLevelData = new BeatmapLevelData(audioClip, difficultyBeatmapSets);
		if (this._difficultyBeatmapSets == null)
		{
			return;
		}
		List<BeatmapCharacteristicSO> list = new List<BeatmapCharacteristicSO>();
		foreach (BeatmapLevelSO.DifficultyBeatmapSet difficultyBeatmapSet in this._difficultyBeatmapSets)
		{
			difficultyBeatmapSet.SetParentLevel(this);
			list.Add(difficultyBeatmapSet.beatmapCharacteristic);
		}
		this._beatmapCharacteristics = list.ToArray();
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x0001AE40 File Offset: 0x00019040
	public virtual void OnEnable()
	{
		this.InitData();
	}

	// Token: 0x04000690 RID: 1680
	[SerializeField]
	protected string _levelID;

	// Token: 0x04000691 RID: 1681
	[SerializeField]
	protected string _songName;

	// Token: 0x04000692 RID: 1682
	[SerializeField]
	protected string _songSubName;

	// Token: 0x04000693 RID: 1683
	[SerializeField]
	protected string _songAuthorName;

	// Token: 0x04000694 RID: 1684
	[SerializeField]
	protected string _levelAuthorName;

	// Token: 0x04000695 RID: 1685
	[SerializeField]
	protected AudioClip _audioClip;

	// Token: 0x04000696 RID: 1686
	[SerializeField]
	protected float _beatsPerMinute;

	// Token: 0x04000697 RID: 1687
	[SerializeField]
	protected float _songTimeOffset;

	// Token: 0x04000698 RID: 1688
	[SerializeField]
	protected float _shuffle;

	// Token: 0x04000699 RID: 1689
	[SerializeField]
	protected float _shufflePeriod;

	// Token: 0x0400069A RID: 1690
	[SerializeField]
	protected float _previewStartTime;

	// Token: 0x0400069B RID: 1691
	[SerializeField]
	protected float _previewDuration;

	// Token: 0x0400069C RID: 1692
	[SerializeField]
	protected Texture2D _coverImageTexture2D;

	// Token: 0x0400069D RID: 1693
	[SerializeField]
	protected SceneInfo _environmentSceneInfo;

	// Token: 0x0400069E RID: 1694
	[SerializeField]
	protected BeatmapLevelSO.DifficultyBeatmapSet[] _difficultyBeatmapSets;

	// Token: 0x0400069F RID: 1695
	protected BeatmapCharacteristicSO[] _beatmapCharacteristics;

	// Token: 0x040006A0 RID: 1696
	protected IBeatmapLevelData _beatmapLevelData;

	// Token: 0x040006A1 RID: 1697
	protected GetBeatmapLevelDataResult _getBeatmapLevelDataResult;

	// Token: 0x020001CE RID: 462
	[Serializable]
	public class DifficultyBeatmapSet : IDifficultyBeatmapSet
	{
		// Token: 0x1700015A RID: 346
		// (get) Token: 0x06000783 RID: 1923 RVA: 0x0001AE48 File Offset: 0x00019048
		public BeatmapCharacteristicSO beatmapCharacteristic
		{
			get
			{
				return this._beatmapCharacteristic;
			}
		}

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x06000784 RID: 1924 RVA: 0x0001AE50 File Offset: 0x00019050
		public IDifficultyBeatmap[] difficultyBeatmaps
		{
			get
			{
				return this._difficultyBeatmaps;
			}
		}

		// Token: 0x06000785 RID: 1925 RVA: 0x0001AE65 File Offset: 0x00019065
		public DifficultyBeatmapSet(BeatmapCharacteristicSO beatmapCharacteristic, BeatmapLevelSO.DifficultyBeatmap[] difficultyBeatmaps)
		{
			this._beatmapCharacteristic = beatmapCharacteristic;
			this._difficultyBeatmaps = difficultyBeatmaps;
		}

		// Token: 0x06000786 RID: 1926 RVA: 0x0001AE7C File Offset: 0x0001907C
		public virtual void SetParentLevel(IBeatmapLevel level)
		{
			BeatmapLevelSO.DifficultyBeatmap[] difficultyBeatmaps = this._difficultyBeatmaps;
			for (int i = 0; i < difficultyBeatmaps.Length; i++)
			{
				difficultyBeatmaps[i].SetParents(level, this);
			}
		}

		// Token: 0x040006A2 RID: 1698
		[SerializeField]
		protected BeatmapCharacteristicSO _beatmapCharacteristic;

		// Token: 0x040006A3 RID: 1699
		[SerializeField]
		protected BeatmapLevelSO.DifficultyBeatmap[] _difficultyBeatmaps;
	}

	// Token: 0x020001CF RID: 463
	[Serializable]
	public class DifficultyBeatmap : IDifficultyBeatmap
	{
		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06000787 RID: 1927 RVA: 0x0001AEA8 File Offset: 0x000190A8
		public BeatmapDifficulty difficulty
		{
			get
			{
				return this._difficulty;
			}
		}

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06000788 RID: 1928 RVA: 0x0001AEB0 File Offset: 0x000190B0
		public int difficultyRank
		{
			get
			{
				return this._difficultyRank;
			}
		}

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06000789 RID: 1929 RVA: 0x0001AEB8 File Offset: 0x000190B8
		public float noteJumpMovementSpeed
		{
			get
			{
				return this._noteJumpMovementSpeed;
			}
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x0600078A RID: 1930 RVA: 0x0001AEC0 File Offset: 0x000190C0
		public int noteJumpStartBeatOffset
		{
			get
			{
				return this._noteJumpStartBeatOffset;
			}
		}

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x0600078B RID: 1931 RVA: 0x0001AEC8 File Offset: 0x000190C8
		public BeatmapData beatmapData
		{
			get
			{
				return this._beatmapData.beatmapData;
			}
		}

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x0600078C RID: 1932 RVA: 0x0001AED5 File Offset: 0x000190D5
		public IBeatmapLevel level
		{
			get
			{
				return this._parentLevel;
			}
		}

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x0600078D RID: 1933 RVA: 0x0001AEDD File Offset: 0x000190DD
		public IDifficultyBeatmapSet parentDifficultyBeatmapSet
		{
			get
			{
				return this._parentDifficultyBeatmapSet;
			}
		}

		// Token: 0x0600078E RID: 1934 RVA: 0x0001AEE8 File Offset: 0x000190E8
		public virtual void SetParents(IBeatmapLevel parentLevel, IDifficultyBeatmapSet parentDifficultyBeatmapSet)
		{
			this._parentLevel = parentLevel;
			this._parentDifficultyBeatmapSet = parentDifficultyBeatmapSet;
			if (this._difficulty == BeatmapDifficulty.ExpertPlus)
			{
				this._beatmapData.SetRequiredDataForLoad(this._parentLevel.beatsPerMinute, 0f, 0f);
				return;
			}
			this._beatmapData.SetRequiredDataForLoad(this._parentLevel.beatsPerMinute, parentLevel.shuffle, parentLevel.shufflePeriod);
		}

		// Token: 0x0600078F RID: 1935 RVA: 0x0001AF4F File Offset: 0x0001914F
		public DifficultyBeatmap(IBeatmapLevel parentLevel, BeatmapDifficulty difficulty, int difficultyRank, BeatmapDataSO beatmapData) : this(parentLevel, difficulty, difficultyRank, 0f, 0, beatmapData)
		{
		}

		// Token: 0x06000790 RID: 1936 RVA: 0x0001AF64 File Offset: 0x00019164
		public DifficultyBeatmap(IBeatmapLevel parentLevel, BeatmapDifficulty difficulty, int difficultyRank, float noteJumpMovementSpeed, int noteJumpStartBeatOffset, BeatmapDataSO beatmapData)
		{
			this._parentLevel = parentLevel;
			this._difficulty = difficulty;
			this._difficultyRank = difficultyRank;
			this._noteJumpMovementSpeed = noteJumpMovementSpeed;
			this._noteJumpStartBeatOffset = noteJumpStartBeatOffset;
			this._beatmapData = beatmapData;
			if (this._difficulty == BeatmapDifficulty.ExpertPlus)
			{
				this._beatmapData.SetRequiredDataForLoad(this._parentLevel.beatsPerMinute, 0f, 0f);
				return;
			}
			this._beatmapData.SetRequiredDataForLoad(this._parentLevel.beatsPerMinute, parentLevel.shuffle, parentLevel.shufflePeriod);
		}

		// Token: 0x040006A4 RID: 1700
		[SerializeField]
		protected BeatmapDifficulty _difficulty;

		// Token: 0x040006A5 RID: 1701
		[SerializeField]
		protected int _difficultyRank;

		// Token: 0x040006A6 RID: 1702
		[SerializeField]
		protected float _noteJumpMovementSpeed;

		// Token: 0x040006A7 RID: 1703
		[SerializeField]
		protected int _noteJumpStartBeatOffset;

		// Token: 0x040006A8 RID: 1704
		[SerializeField]
		protected BeatmapDataSO _beatmapData;

		// Token: 0x040006A9 RID: 1705
		protected IBeatmapLevel _parentLevel;

		// Token: 0x040006AA RID: 1706
		protected IDifficultyBeatmapSet _parentDifficultyBeatmapSet;
	}*/
}
