This app is configurable. You are able to set values like the size of the cards, the fonts, where the app should read from for the prompt and answer cards, and where it should save the ouptut images.

All of these configurations are in AdinaCardGame.exe.config. You can change the value in quotes for each of these to your desired setting.

Configurable values:

    CardWidthInInches
    CardHeightInInches
        These values are the actual size of the cards in inches. For example, standard poker cards are 2.5 x 3.5 inches.
    
    BleedSizeInInches
        This value is the extra space to leave around the outside of the card, in inches. 

    PromptFontFamily
    AnswerFontFamily
        These are the fonts used for both types of cards. Any font installed on your machine should work.

    BorderRadius
        This value controls the radius of the rounded corners. It doesn't matter much because they get cut by the printer anyway. 
    
    BorderPadding
        This value controls how much space is between the side of the card and the text. You can play with this value to get it looking the way you want. 
    
    MaxPromptTextFontSize
    MaxAnswerTextFontSize
        These two values control the max font size for each type of card. It will use a smaller font size when necessary, though. 
    
    
    PromptCardFrontBackgroundColor
    PromptCardFrontTextColor
    AnswerCardFrontBackgroundColor
    AnswerCardFrontTextColor
        These four values control the colors of the background and text of each card.
        They are RGB values - you can use any RGB color picker you find online to use them.
        0, 0, 0 is black and 255, 255, 255 is white. 255, 0, 0  is an intense shade of red, for instance.
        Eacg value for each of the 3 numbers can be between 0 and 255. 
    
    AnswerCardPath
    PromptCardPath
        These 2 paths control where your answer and prompt card files are.
        You need to change them to your prompt and answer files for the app to read.

        Some more explanation about formatting these files:
            These files each have one entry per line. They ignore any extra blank lines. 
            For the prompt cards (the black cards with blanks), you can have the app fill up entire lines
            automatically with ____'s. To do this, surround the part of the line with @'s, and include a single _.
            For instance:
            @Step 1: _.@ @Step 2: _.@ Step 3: Profit
            Will have 3 lines on the card:
            Step 1: _______.
            Step 2: _______.
            Step 3: Profit
            
            and the app will make sure that the ___'s fill up the line. 
            
            A second example:
            Instead of coal, Santa now gives the bad children @_.@
            Will print:
            Instead of coal, Santa now gives the bad children
            __________.
            
            The first part (Instead of... ) will wrap onto as many lines as needed. The second part will always be on its own line.
            
            Third example:
            TSA guidelines now prohibit @_@ on airplanes.
            
            Will print:
            TSA guidelines now prohibit
            _________
            on airplanes.
            
            The app also ignores any lines that start with //.
    

    OutputPath
        This is where the app will drop the files. It will create a subfolder with the current date and time. 

    
    TemplatePath
    OverlayTemplate
    
        These control a useful feature to overlay a template from thegamecrafter on top of your image.
        
        To use it, download the template (in png form) for the card you're using. 
        Set the path below to where you downloaded it to.
        Set the second value to true.
        
        You'll see what it does. 