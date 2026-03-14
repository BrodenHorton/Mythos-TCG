public class ConsoleRichTextTestCommand : Command {
    private string testStr = "&1Test 1 &2Test 2 &3Test 3 &4Test 4 &5Test 5 &6Test 6 &7Test 7 &8Test 8 &9Test 9 &0Test 0 &aTest a &bTest b &cTest c &dTest d &eTest e &fTest f";

    public ConsoleRichTextTestCommand() {
        cmdName = "testRichText";
    }

    public override void Execute(string[] args) {
        TcgLogger.Log(testStr);
    }
}