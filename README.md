# Bonaventure Capitalism

**Product Version:** v0.4.0
**Project Title:** Bonaventure Capitalism

## Overview
Bonaventure Capitalism v0.4.0 delivers a **playable MVP prototype** of a financial simulation game. Players manage a portfolio, navigate a dynamic economy, and make strategic investments, with all core mechanics integrated to provide a complete decision-based learning experience. The game emphasises consequences over overt teaching, allowing players to learn through simulation and experimentation as they strive to achieve a significant financial goal.

## The Journey Begins: Backstory
You embark on your entrepreneurial quest with a $10,000 inheritance, a final heartfelt gift from your late grandfather. His dying wish was for you to take this initial capital and grow it into a million-dollar empire, securing his legacy and proving your strategic prowess. The market awaits, and your adventure to financial success begins now.

## Key Features of the MVP (v0.4.0)

### Core Game Flow & Progression
* **Player Onboarding:** Start with $10,000 inherited capital and a clear objective.
* **Company Selection:** Choose from distinct company types, each offering unique starting perks.
* **Time Progression:** A functional game clock (days, weeks, months) that drives economic cycles.
* **Win/Loss Conditions:** The game concludes upon reaching a net worth of $1,000,000 (Win) or falling into bankruptcy (Loss).

### Economic & Financial Systems
* **Dynamic Stock Market Simulation:** A simplified yet realistic market with rising/falling trends and elements of randomness affecting stock prices.
* **Comprehensive Budget Dashboard:** A centralised interface displaying vital financial stats including cash, income, expenses, credit, debt, savings, and overall cash flow.
* **Interactive Loan/Credit System:** Players can take loans with interest and manage time-based repayments.
* **Player Portfolio Management:** Tracks all player investments, calculating current value and performance.

### Engagement & Learning Mechanics
* **Randomised Event System:** Real-world-style economic and market events (e.g., market crashes, booms) dynamically affect gameplay, requiring adaptive decision-making.
* **Embedded Learning:** Financial principles are learned through direct consequences of investment and management choices, rather than explicit instruction.
* **Achievement Tracking:** Milestones are tracked and rewarded (e.g., "First $50K Net Worth," "Survived a Crash").
* **Glossary Panel:** An in-game glossary provides definitions for key financial terms, loaded from an external JSON file.
* **Player XP System:** Basic experience point tracking for future expansion of progression.

### User Interface & Feedback
* **Intuitive UI:** A functional finance interface provides clear visual feedback on all financial data and market trends.
* **Popup System:** A robust system for displaying messages, confirmations, and narrative elements to the player.
* **News Ticker:** Delivers timely updates and narrative flavor.

## Core Systems & Scripts

### Game Management & Flow
-   `GameManager.cs`: Central controller for game flow, initialisation, state transitions, company perk application, and initial capital/backstory prompt.
-   `StartupManager.cs`: Handles initial scene loading and game setup.
-   `TimeManager.cs`: Manages time progression and triggers game updates.
-   `TimeControlUI.cs`: Manages UI for controlling game time (pause, speed up).
-   `WinLossChecker.cs`: Implements the logic for checking and concluding game based on win/loss conditions.
-   `VersionInfo.cs`, `VersionLoader.cs`: Manages and displays the game version information.

### Player & Company
-   `PlayerStats.cs`: Stores core financial attributes (cash, net worth).
-   `PlayerCompany.cs`: Manages the player's chosen company type and associated perks.
-   `CompanyType.cs`: Defines available company types.
-   `CompanySelectionPanel.cs`: Handles UI for company selection.
-   `PlayerXP.cs`: Manages player experience points.

### Stock Market & Investment
-   `StockMarketSystem.cs`: Updates stock prices, manages stock data.
-   `Stock.cs`: Defines individual stock properties.
-   `StockTradeSystem.cs`: Manages player's stock buying and selling.
-   `StockUIEntry.cs`: UI representation for individual stocks.
-   `PortfolioSystem.cs`: Tracks player's stock holdings and portfolio performance.

### Finance Mechanics
-   `CreditSystem.cs`: Manages loans, interest, and debt.
-   `BudgetSystem.cs`: Handles income, expenses, and cash flow.

### Random Events & Narrative
-   `EventSystem.cs`: Triggers and applies effects of random events (loaded from `events.json`).
-   `EventData.cs`: Defines event data structure.
-   `NewsTicker.cs`: Displays in-game news.

### User Interface
-   `UIsidebarController.cs`: Manages navigation between main UI panels.
-   `DashboardUI.cs`: Displays financial overview.
-   `FinanceDashboardPanel.cs`: Detailed finance statistics panel.
-   `Popup.cs`: Generic and reusable popup window.
-   `PopupManager.cs`: Manages popup display queue.

### Achievements
-   `AchievementSystem.cs`: Manages achievement definitions, tracking, and unlocking (loaded from JSON).
-   `AchievementUI.cs`: Displays achievement status in UI.

### Glossary System
-   `GlossaryManager.cs`: Loads and manages glossary data from JSON.
-   `GlossaryData.cs`: Defines glossary entry structure.
-   `GlossaryUI.cs`: Displays glossary content in UI.

### Visualisation & Debug
-   `GraphPlotter.cs`: Used for plotting data, such as stock price trends. (Buggy)
-   `DebugStockUpdater.cs`, `TestCashController.cs`: (Located in `Test Tools` folder) Utility scripts for debugging and testing purposes.
