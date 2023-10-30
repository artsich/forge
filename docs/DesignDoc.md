# Game Design Document for "Fire Keeper"

**Title**: Fire Keeper  
**Genre**: Survival  
**Platform**: [Windows, MacOS]  
**Visual Style**: 2D top-down perspective(ortho?)  

## Game Overview
"Fire Keeper" is a 2D top-down survival game where the player is tasked with maintaining a fire in a dense forest. The primary objective is to keep the fire burning by gathering wood and protecting it from the elements. If the fire goes out, the player loses.

## Gameplay Mechanics

### Movement
- The player can move freely in all four directions within the forest environment.

### Wood Chopping (Рубка дров)
- Players can chop down trees to gather wood. This action is performed by approaching a tree and pressing btn.

### Branch Searching (Поиск веток)
- The forest floor is littered with branches that can be collected for kindling.

### Fire Interaction (Взаимодействие с огнем)
- Players can add wood to the fire to keep it burning. Interaction is intuitive, involving dragging and dropping wood onto the fire.

### Fire Animation
- The fire will be represented with a dynamic 2D animation that reflects its current state (intensity, size, etc.).

### Rain Event
- Random rain events will occur, threatening to extinguish the fire. The player must move the fire or protect it by building a shelter.

### Save/Load System
- Players can save their progress and load from the last checkpoint.

## Technical Specifications

### Art & Animation
- The game will feature stylized 2D graphics with smooth animations for the player, environment, and fire.
- The fire animation will be particularly detailed, with frames that show varying intensities and behaviors (e.g., crackling, sparks).

### Sound
- Ambient forest sounds, chopping noises, fire crackling, and weather effects will be included to enhance immersion.

### User Interface
- A minimal UI will display the current state of the fire, inventory items like wood, and a weather indicator when rain is approaching.

### Programming
- The game will be built using Forge GE.
- Physics for movement, collision detection for trees and fire interaction.
- Weather system to simulate rain events and their impact on the fire.

## Milestones
- **Concept Development**: Complete the game concept and core mechanics.
- **Prototype**: A basic playable version showcasing the movement, wood chopping, and fire mechanics.
- **Alpha Release**: Integrate all core mechanics with placeholder graphics.
- **Beta Release**: Complete game with full graphics and sound, ready for testing.
- **Final Release**: After testing and polishing, the game will be ready for launch.
