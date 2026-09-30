---
inclusion: auto
name: git-assets
description: Git, Unity asset, prefab, scene, meta file and Git LFS rules.
---

# Git and Unity Asset Rules

## Unity metadata

Commit `.meta` files together with their corresponding assets.

Never manually regenerate or delete `.meta` files without understanding the reference impact.

## Binary assets

Use Git LFS for large binary assets such as high-resolution textures, audio, video and source art when appropriate.

## Scenes and Prefabs

Prefer small focused prefabs and additive composition over oversized prefabs with unrelated responsibilities.

Avoid unnecessary scene rewrites that create large merge conflicts.

## Versioned project settings

Project settings required for reproducible builds should be committed.

Machine-specific generated state should not be committed.
