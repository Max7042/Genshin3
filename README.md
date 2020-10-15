# GenshinOverlay

An application for displaying an overlay over the Genshin Impact window which visually displays cooldown timers for skills.

The majority of what is explained below can also simply be found in GenshinOverlay as tooltips when you hover over things.

# Getting Started

- Start GenshinOverlay as Administrator
- Click the "Configure Overlay" button while Genshin Impact is running.
  - Note: Genshin Impact must be running in a window, fullscreen is not supported.
  - If your resolution is supported (below 1920x1080): GenshinOverlay should automatically preset values.
  - If your resolution isn't supported: You will need to configure the position of things yourself, check the sections below.
  - Before getting started with making adjustments, do note that you can make more precise changes by scrolling or
    using the arrow keys on your keyboard while hovering over a trackbar.
  - After you are done making adjustments, click the Save Configuration button, then whenever the game is active & unpaused, you should see your cooldown bars.

# Configure Overlay - Cooldown Text
 The Cooldown Text is the value displayed counting down in the bottom right of the game window after you use a skill.
  - This text is used by the OCR engine to detect the current cooldown value.
  - Adjust Cooldown Text "**X Pos**", "**Y Pos**", "**Width**" & "**Height**" until you have a red rectangle enveloping the Cooldown Text.
  - Optimally, the rectangle should be within the circular area, but also have a bit of spacing from the cooldown value,
    you should have something like the image below.
  - ![1](https://i.imgur.com/EGKuE3J.png)
  
# Configure Overlay - Party Numbers
 The Party Numbers are displayed on the very right of the game window.
  - These party numbers are used to determine which character is currently selected.
  - Adjust Party Numbers "**X Pos**", "**Y Pos**", "**Y Offset**" until you have 4 red dots within the white area of the Party Numbers.
  - It's very important that these red dots are on the white area, not overlapping with the numbers, 
    you should have something like the image below.
  - ![2](https://i.imgur.com/Ee15GyG.png)
  
# Configure Overlay - Cooldown Bars
  - All of these values depend on your personal preference for setting up the appearance of cooldown bars.
  
# Configure Overlay - Cooldown Override
 This section allows you to define a specific cooldown value for each character, this is necessary for characters like Fischl because her actual cooldown isn't presented instantly - she instead has a 2sec cooldown for repositioning Oz.
  - For characters like Fischl, set the cooldown value of their party slot to roughly the value of their total cooldown time (found under Talent Info in-game).
  
# Cooldown Properties
 This section provides some variables that you probably won't need to change, but they're there for fine-tuning.
 - **Max** - This defines the max possible assumed cooldown, just in case the OCR engine makes an odd mistake.
   This value should be no less than the character with the highest cooldown in-game.
 - **Offset** - This allows you to define an offset amount from the assumed cooldown, this is useful if the cooldown bar completes too soon or too late compared to actual cooldown.
 - **Reapply** - This defines the minimum cooldown time remaining before pressing the skill key will trigger OCR detection, this makes up for situations where the actual cooldown ends prior to the assumed cooldown.
 - **Override** - This determines the max cooldown time remaining to consider overridable, this is useful for multi-party situations where you have a character in the same slot as a character like Fischl, but you don't want their cooldown to also be overridden. The value of this should be a little more than the value of **Reapply**.
 - **Pause Sub** - This is a value to subtract the remaining cooldown time by whenever the game is paused to make up for frames where cooldown isn't detected after an unpause.
 - **Tick Rate** - This is the rate at which cooldown will be calculated and cooldown bars will update.
 - **Confidence** - This is the minimum confidence that the OCR engine must have before returning a cooldown value.
 
# Auto Setup
 This enables you to automatically update the values of things if you ever switch between resolutions.
 - You can add new resolutions to the file **GenshinOverlay.Templates.ini**
 - Note: Adding or making changes to resolutions in this file will required GenshinOverlay to be restarted.
