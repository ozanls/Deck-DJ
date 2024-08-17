# Deck-DJ


The YGO Consistency Calculator aims to simplify the calculations to maximize the consistency
of players' decks allowing players to make meaningful adjustments and increase their
competitive success. Users will be able to build decks within the application.
Soundbyte is an application that allows users to upload and share music and audio. Users can
upload a sound clip and assign a genre to them.
Deck DJ is the combination of these two products, by associating different genres of music to
specific card types found in Yu-Gi-Oh, once users have created a deck, they will also have a
random playlist generated based on the cards found in their deck.

##Features
  - Admin Functionality
    - Cards and Categories are Read-Only for non-admin users 
  - File upload for audio
  - Pagination for Cards

##Examples
### Create Card
Creating a card with use the API endpoint /api/CardData/AddCard to add a card into the database
![Create a Card WebPage](/assets/create.png)

### Viewing Deck Details
Get the details of a deck with the API endpoint /api/DeckData/
![View Deck WebPage](/assets/details.png)

## Work Distribution:

### Kwasi:
  
  - Listing audio associated with specific decks  
  - Displaying that on the Deck Details page  
  - Associating audio with decks  
  - Final Debugging
  - Pagination
  - Admin

### Ozan:  
  - Intitial Combination of Passion Projects  
  - Listing decks associated with specific audio  
  - Displaying that to the Audio Details page  
  - Unassociating audio with decks
  - File Upload
  
