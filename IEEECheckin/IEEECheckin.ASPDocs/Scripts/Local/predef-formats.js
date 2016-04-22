var predefThemes = [
    { "name": "UMN IEEE", "theme": "{\"bodyBackgroundColor\":\"006699\", \"buttonBackgroundColor\":\"39b3d7\", \"bodyColor\":\"ffffff\", \"themeShade\":\"light\", \"imageUrl\":\"../Images/logo.svg\", \"headerText\":\"University of Minnesota\", \"useSwipe\":\"true\"}" },
    { "name": "UMN IEEE WIE", "theme": "{\"bodyBackgroundColor\":\"a058c9\",\"buttonBackgroundColor\":\"5f0594\",\"bodyColor\":\"ffffff\",\"themeShade\":\"light\",\"imageUrl\":\"http://ieee.griet.ac.in/wp-content/uploads/2014/04/ieee_wie_purple.png\",\"headerText\":\"University of Minnesota\",\"useSwipe\":\"true\"}" },
    { "name": "GOFIRST", "theme": "{\"bodyBackgroundColor\":\"ffd018\",\"buttonBackgroundColor\":\"9e1313\",\"bodyColor\":\"000000\",\"themeShade\":\"dark\",\"imageUrl\":\"http://www.mngofirst.org/uploads/6/3/5/8/6358288/1401824902.png\",\"headerText\":\"GOFIRST\",\"useSwipe\":\"true\"}" }
];

var predefRegex = [
    { "name": "University of Minnesota", "regex": "{\"regex\": \"^%(\\w+)\\^(\\d+)\\^{3}(\\d+)\\^(\\w+),\\s(?:([\\w\\s]+)\\s(\\w{1})\\?;|([\\w\\s]+)\\?;)(\\d+)=(\\d+)\\?$\", \"indices\": {\"firstName\":\"5,7\",\"lastName\":\"4\",\"middleName\":\"6\",\"studentId\":\"2\",\"email\":\"-1\"}, \"name\": \"University of Minnesota\"}" }
];