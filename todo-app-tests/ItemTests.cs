using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq.EntityFrameworkCore;
using todo_aspnetmvc_ui.Controllers;

namespace todo_app_tests;

[TestFixture]
public class ItemTests
{
    private Mock<IItemService> _mockItemService;
    private Mock<IListService> _mockListService;
    private ItemController _controller;
    private ItemService _itemService;
    private Mock<TodoContext> _mockContext;

    [SetUp]
    public void Setup()
    {
        _mockItemService = new Mock<IItemService>();
        _mockListService = new Mock<IListService>();
        _controller = new ItemController(_mockItemService.Object, _mockListService.Object);
        _mockContext = new Mock<TodoContext>();
        _itemService = new ItemService(_mockContext.Object);
    }

    [Test]
    public async Task Index_WithListId_ReturnsViewWithItemsByListId()
    {
        // Arrange
        int listId = 1;
        var items = new List<TodoItem>
        {
            new TodoItem { Id = 1, Title = "Item 1" },
            new TodoItem { Id = 2, Title = "Item 2" }
        };
        _mockItemService.Setup(s => s.GetItemsByListId(listId)).ReturnsAsync(items);

        // Act
        var result = await _controller.Index(listId) as ViewResult;

        // Assert
        result.ShouldNotBeNull();
        result.Model.ShouldBe(items);
    }

    [Test]
    public async Task Index_WithoutListId_ReturnsViewWithAllItems()
    {
        // Arrange
        var items = new List<TodoItem>
        {
            new TodoItem { Id = 1, Title = "Item 1" },
            new TodoItem { Id = 2, Title = "Item 2" }
        };
        _mockItemService.Setup(s => s.GetItems()).ReturnsAsync(items);

        // Act
        var result = await _controller.Index(null) as ViewResult;

        // Assert
        result.ShouldNotBeNull();
        result.Model.ShouldBe(items);
    }
    
    [Test]
    public async Task GetItems_WithoutListId_ReturnsListWithAllItems()
    {
        // Arrange
        var items = new List<TodoItem>
        {
            new() { Id = 1, Title = "Item 1" },
            new() { Id = 2, Title = "Item 2" }
        };
        var mockContext = new Mock<TodoContext>();
        mockContext.Setup(ctx => ctx.TodoItems).ReturnsDbSet(items);
        var itemService = new ItemService(mockContext.Object);
        
        // Act
        var result = await itemService.GetItems();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(items);
    }

    [Test]
    public async Task Details_WithValidId_ReturnsViewWithTodoItem()
    {
        // Arrange
        int itemId = 1;
        var todoItem = new TodoItem { Id = itemId, Title = "Item 1" };
        _mockItemService.Setup(s => s.GetItemById(itemId)).ReturnsAsync(todoItem);

        // Act
        var result = await _controller.Details(itemId) as ViewResult;

        // Assert
        result.ShouldNotBeNull();
        result.Model.ShouldBe(todoItem);
    }

    [Test]
    public async Task Details_WithNullId_ReturnsNotFound()
    {
        // Arrange & Act
        var result = await _controller.Details(null);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Test]
    public async Task Create_Get_ShowsListsViewData()
    {
        // Arrange
        var lists = new List<TodoList>
        {
            new() { Id = 1, Title = "List 1" },
            new TodoList { Id = 2, Title = "List 2" }
        };
        _mockListService.Setup(s => s.GetLists()).ReturnsAsync(lists);

        // Act
        var result = await _controller.Create() as ViewResult;

        // Assert
        result.ShouldNotBeNull();
        result.ViewData["ToDoListId"].ShouldBeOfType<SelectList>();
    }
    
    [Test]
    public async Task Create_Post_RedirectsToIndex()
    {
        // Arrange
        var todoItem = new TodoItem { Title = "Valid Item" };

        // Act
        var result = await _controller.Create(todoItem) as RedirectToActionResult;

        // Assert
        result.ShouldNotBeNull();
        result.ActionName.ShouldBe(nameof(ItemController.Index));
    }

    [Test]
    public async Task Edit_ValidId_ReturnsViewWithCorrectData()
    {
        // Arrange
        int id = 5;
        var expectedItem = new TodoItem { Id = id, Title = "Item 5" };

        _mockItemService.Setup(s => s.GetItemById(id)).ReturnsAsync(expectedItem);

        // Act
        var result = await _controller.Edit(id) as ViewResult;

        // Assert
        result.ShouldNotBeNull();
        result.Model.ShouldBe(expectedItem);
    }

    [Test]
    public async Task Edit_NullId_InvalidId_ReturnsNotFound()
    {
        // Arrange
        int? id = null;
        const int id2 = 100;
        TodoItem nullItem = null;
        _mockItemService.Setup(s => s.GetItemById(id2))!.ReturnsAsync(nullItem);

        // Act
        var result = await _controller.Edit(id);
        var result2 = await _controller.Edit(id);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<NotFoundResult>();
        result2.ShouldNotBeNull();
        result2.ShouldBeOfType<NotFoundResult>();
    }

    [Test]
    public async Task Edit_ValidIdAndTodoItem_ReturnsRedirectToActionResult()
    {
        // Arrange
        var id = 5;
        var todoItem = new TodoItem { Id = id, Title = "Updated Item", Status = State.Completed };

        // Act
        var result = await _controller.Edit(id, todoItem) as RedirectToActionResult;

        // Assert
        result.ShouldNotBeNull();
        result.ActionName.ShouldBe(nameof(Index));
    }

    [Test]
    public async Task Edit_InvalidId_ReturnsNotFoundResult()
    {
        // Arrange
        var id = 5;
        var todoItem = new TodoItem { Id = id + 1, Title = "Updated Item", Status = State.Completed };

        // Act
        var result = await _controller.Edit(id, todoItem);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Test]
    public async Task Edit_DbUpdateConcurrencyException_ReturnsNotFoundResult()
    {
        // Arrange
        var id = 5;
        var todoItem = new TodoItem { Id = id, Title = "Updated Item", Status = State.Completed };

        _mockItemService.Setup(s => s.UpdateItem(todoItem)).ThrowsAsync(new DbUpdateConcurrencyException());

        // Act
        var result = await _controller.Edit(id, todoItem);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Test]
    public async Task Delete_ValidId_ReturnsViewResultWithTodoItem()
    {
        // Arrange
        var id = 5;
        var todoItem = new TodoItem { Id = id, Title = "Item to Delete" };
        _mockItemService.Setup(s => s.GetItemById(id)).ReturnsAsync(todoItem);

        // Act
        var result = await _controller.Delete(id) as ViewResult;

        // Assert
        result.ShouldNotBeNull();
        result.ViewData.Model.ShouldBe(todoItem);
    }

    [Test]
    public async Task Delete_NullId_ReturnsNotFound()
    {
        // Arrange
        int? id = null;

        // Act
        var result = await _controller.Delete(id);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }
    
    [Test]
    public async Task GetItemById_InvalidId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var id = -1;

        // Act and Assert
        await Should.ThrowAsync<ArgumentOutOfRangeException>(async () =>
        {
            await _itemService.GetItemById(id);
        });
    }

    [Test]
    public async Task AddItem_NullItem_ThrowsArgumentNullException()
    {
        // Arrange
        TodoItem item = null;

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () => await _itemService.AddItem(item));
    }

    [Test]
    public async Task UpdateItem_InvalidId_ThrowsInvalidOperationException()
    {
        // Arrange
        var item = new TodoItem { Id = 0, Title = "Item 1", Status = State.Completed };

        // Act and Assert
        await Should.ThrowAsync<InvalidOperationException>(async () => await _itemService.UpdateItem(item));
    }

    [Test]
    public async Task DeleteItem_InvalidId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const int id = -1;

        // Act and Assert
        await Should.ThrowAsync<ArgumentOutOfRangeException>(async () =>
        {
            await _itemService.DeleteItem(id);
        });
    }
}
