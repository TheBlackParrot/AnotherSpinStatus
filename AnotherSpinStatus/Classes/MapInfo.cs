using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace AnotherSpinStatus.Classes;

public struct MapInfo
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private enum DlcAbbreviations
    {
        BG = 0, // Base game
        MC = 1000, // Monstercat
        CH = 2000, // Chillhop
        SP = 3000, // Supporter Pack
        IN = 4000 // Indie Pack
    }
    private static readonly Dictionary<DlcAbbreviations, string> DlcNames = new()
    {
        { DlcAbbreviations.BG, "Base Game" },
        { DlcAbbreviations.MC, "Monstercat DLC" },
        { DlcAbbreviations.CH, "Chillhop DLC" },
        { DlcAbbreviations.SP, "Supporter Pack DLC" },
        { DlcAbbreviations.IN, "Indie Pack DLC" }
    };
    
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // ReSharper disable MemberCanBePrivate.Global
    public string Title { get; }
    public string Subtitle { get; }
    public string Artist { get; }
    public string Charter { get; }
    public bool IsCustom { get; }
    public string? Pack { get; set; }
    public string? FileReference { get; }
    public string Difficulty { get; }
    public int Rating { get; }
    public int Duration { get; }
    public string? CoverArt { get; }
    // ReSharper restore MemberCanBePrivate.Global
    // ReSharper restore UnusedAutoPropertyAccessor.Global
    
    public MapInfo(PlayableTrackData trackData)
    {
        TrackInfoMetadata metadata = trackData.Setup.TrackDataSegmentForSingleTrackDataSetup.metadata.TrackInfoMetadata;
        MetadataHandle metadataHandle = trackData.Setup.TrackDataSegmentForSingleTrackDataSetup.metadata;
        
        Title = metadata.title;
        Subtitle = metadata.subtitle;
        Artist = metadata.artistName;
        Charter = metadata.isCustom
            ? metadata.charter
            : (trackData.Difficulty == TrackData.DifficultyType.RemiXD ? metadata.charter : string.Empty);
        Difficulty = trackData.Difficulty.ToString();
        Rating = trackData.DifficultyRating;
        IsCustom = metadata.isCustom;
        Pack = IsCustom ? null : DlcNames[(DlcAbbreviations)metadata.trackOrder - (metadata.trackOrder % 1000)];
        Duration = trackData.GameplayEndTick.ToSecondsInt();
        
        string? reference = metadataHandle.UniqueName;
        if (!string.IsNullOrEmpty(reference))
        {
            if (reference.LastIndexOf('_') != -1)
            {
                reference = reference.Remove(metadataHandle.UniqueName.LastIndexOf('_')).Replace("CUSTOM_", string.Empty);
            }
        }
        FileReference = reference;

        // referenced SpinStatus's methods for getting cover art after being unable to figure out what was going on myself
        Texture2D? cover = null;
        try
        {
            if (IsCustom)
            {
                string filename = Path.Combine(Plugin.CustomDataPath, "AlbumArt", $"{FileReference}.png");
                cover = new Texture2D(1, 1);
                cover.LoadImage(File.ReadAllBytes(filename));
            }
            else
            {
                Texture2DAssetReference coverReference = metadataHandle.albumArtRef;
                AssetBundle assetBundle = AssetBundle.GetAllLoadedAssetBundles()
                    .First(bundle => bundle.name == coverReference.Bundle);
                cover = assetBundle.LoadAsset<Texture2D>(coverReference.Guid);
            }
        }
        catch (Exception e)
        {
            Plugin.Log.LogWarning(e);
        }

        if (cover == null)
        {
            return;
        }
        
        RenderTexture renderTexture = RenderTexture.GetTemporary(Plugin.CoverArtSize.Value, Plugin.CoverArtSize.Value);
        Graphics.Blit(cover, renderTexture);
        
        Texture2D finalCover = new(renderTexture.width, renderTexture.height);
        finalCover.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        finalCover.Apply();
        RenderTexture.ReleaseTemporary(renderTexture);
        
        CoverArt = Convert.ToBase64String(ImageConversion
            .EncodeNativeArrayToJPG(finalCover.GetRawTextureData<byte>(), finalCover.graphicsFormat,
                (uint)finalCover.width, (uint)finalCover.height, 0u, Plugin.CoverArtQuality.Value).ToArray());
    }
}