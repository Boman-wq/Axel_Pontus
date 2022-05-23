using Microsoft.VisualStudio.TestTools.UnitTesting;
using Catalog.Reposotories;
using Catalog.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Catalog.Controllers;
using System.Threading.Tasks;
using Moq;
using Microsoft.AspNetCore.Hosting;
using FluentAssertions;
using System;
using Microsoft.Extensions.Logging;
using Catalog.Dtos;

namespace WebAPITest
{
    [TestClass]
    public class ApiTest
    {
        private readonly GameController _sut;
        private readonly Mock<IGameRepository> _gameRepoMock = new Mock<IGameRepository>();
        private readonly Mock<ILogger<GameController>> _loggerMock = new Mock<ILogger<GameController>>();
        private readonly Random rand = new();

        public ApiTest()
        {
            _sut = new GameController(_gameRepoMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task GetGame_WithUnexistingItem_ShouldReturnNotFound()
        {
            // Arrange
            _gameRepoMock.Setup(x => x.GetGame(It.IsAny<Guid>()))
                .ReturnsAsync((Game)null);

            // Act
            var result = await _sut.GetGame(It.IsAny<Guid>());

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public async Task GetGame_WithExistingItem_ShouldReturnExpectedItem()
        {
            // Arrange
            Game expectedGame = CreateRandomGame();

            _gameRepoMock.Setup(x => x.GetGame(It.IsAny<Guid>())).ReturnsAsync(expectedGame);

            // Act
            var result = await _sut.GetGame(Guid.NewGuid());

            // Assert
            result.Value.Should().BeEquivalentTo(expectedGame);
        }
        [TestMethod]
        public async Task GetGames_WithExistingItem_ShouldReturnAllItems()
        {
            // Arrange
            var expectedItems = new[] { CreateRandomGame(), CreateRandomGame(), CreateRandomGame(), CreateRandomGame() };

            // Act
            _gameRepoMock.Setup(x => x.GetGames())
                .ReturnsAsync(expectedItems);

            var actualItems = await _sut.GetGames();

            // Assert
            actualItems.Should().BeEquivalentTo(expectedItems);
        }
        [TestMethod]
        public async Task GetGames_WithMatchingItems_ShouldReturnMatchingGames()
        {
            // Arrange
            Game[] games = new[]
            {
                new Game(){Name = "CS:GO"},
                new Game(){Name = "LoL"},
                new Game(){Name = "WoW"}
            };
            var name = "WoW";

            _gameRepoMock.Setup(x => x.GetGames()).ReturnsAsync(games);

            // Act
            IEnumerable<GameDto> foundGames = await _sut.GetGames(name);


            // Assert
            foundGames.Should().OnlyContain( game => game.Name == games[0].Name || game.Name == games[2].Name);

        }
        [TestMethod]
        public async Task CreateGame_WithGameToCreate_ShouldReturnCreatedGame()
        {
            // Arrange
            var gameToCreate = new CreateGameDto(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                rand.Next(1, 10),
                Guid.NewGuid().ToString());

            // Act
            var result = await _sut.CreateGame(gameToCreate);

            // Assert
            var createdGame = (result.Result as CreatedAtActionResult).Value as GameDto;

            gameToCreate.Should().BeEquivalentTo(
                createdGame, op => op.ComparingByMembers<GameDto>().ExcludingMissingMembers()
            );
            createdGame.Id.Should().NotBeEmpty();

        }
        [TestMethod]
        public async Task Updategame_WithExistingGame_ShouldReturnNoContent()
        {
            // Arrange
            var existingGame = CreateRandomGame();

            _gameRepoMock.Setup(x => x.GetGame(It.IsAny<Guid>())).ReturnsAsync(existingGame);

            var gameId = existingGame.Id;
            var gameUpdate = new UpdateGameDto(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                rand.Next(1, 10),
                Guid.NewGuid().ToString());
            // Act
            var result = await _sut.UpdateGame(gameId, gameUpdate);

            // Assert

            result.Should().BeOfType<NoContentResult>();
        }

        [TestMethod]
        public async Task DeleteGames_WithExistingGame_ShouldReturnNoContent()
        {
            //Arrange
            Game existingGame = CreateRandomGame();
            _gameRepoMock.Setup(x => x.GetGame(It.IsAny<Guid>())).ReturnsAsync(existingGame);

            //Act
            var result = await _sut.DeleteGame(existingGame.Id);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }
        
        [TestMethod]
        public async Task SearchGame_WithExistingGame_ShouldReturnGame()
        {
            string gameName = "gameNames";
            List<Game> existingGames = new()
            {
                new Game(){
                    Id = Guid.NewGuid(),
                    Name = gameName,
                    Description = Guid.NewGuid().ToString(),
                    Grade = rand.Next(1, 10),
                    Image = Guid.NewGuid().ToString()
                }
            };

            _gameRepoMock.Setup(x => x.GetGames()).ReturnsAsync(existingGames);

            var result = await _sut.Search(gameName);

            result.Value.Should().NotBeNull();
        }

        [TestMethod]
        public async Task SearchGame_WithUnexistingGame_ShouldReturnNotFound()
        {
            throw new NotImplementedException();
        }

        private Game CreateRandomGame()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Grade = rand.Next(1, 10),
                Image = Guid.NewGuid().ToString(),
            };
        }
    }
}
