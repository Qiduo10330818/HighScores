# HighScores

Getting the tools (最新版的 Firefox 好像不支援 SQLite Manager，我用的是 SQLiteStudio，功能幾乎一樣):
https://www.youtube.com/watch?v=RTFaFWxb1WA&list=PLX-uZVK_0K_7NmsYfe2BTOk_IamWC2kU3&index=2

TO DO :
1. 在client裡加入HighScoresManager(hierarchy裡create一個 Empty Game Object，然後把HighScoresManager.cs丟到裡面)和highscores.db(data base檔案，放在asset資料夾裡)
2. client的canvas要加入scoreboard(有些做好的東西在HighScores的Assets/Prefabs裡面)，這樣才能顯示分數
3. ClientHandle.cs 裡，如果收到ScoreUpdate，要同步去更新highscores.db裡的資料(呼叫HighScoreManager.cs裡的UpdateScore)
4. 所以ScoreUpdate要傳給每個client，並且packet裡要寫上分數被更新的player的ID
