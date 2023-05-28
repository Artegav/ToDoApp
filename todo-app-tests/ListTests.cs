using Microsoft.AspNetCore.Mvc;
using todo_aspnetmvc_ui.Controllers;

namespace todo_app_tests;

public class ListServiceTests
{
    private Mock<IItemService> _mockItemService;
    private Mock<IListService> _mockListService;
    private TodoController _controller;
    private ItemService _itemService;
    private ListService _listService;
    private Mock<TodoContext> _mockContext;
    
    [SetUp]
    public void Setup()
    {
        _mockItemService = new Mock<IItemService>();
        _mockListService = new Mock<IListService>();
        _controller = new TodoController(_mockListService.Object, _mockItemService.Object);
        _mockContext = new Mock<TodoContext>();
        _itemService = new ItemService(_mockContext.Object);
        _listService = new ListService(_mockContext.Object);
    }

    [Test]
    public async Task Index_ReturnsViewWithLists()
    {
        // Arrange
        var lists = new List<TodoList>
        {
            new() { Id = 1, Title = "List 1" },
            new() { Id = 2, Title = "List 2" }
        };
        var items = new List<TodoItem>
        {
            new() { Id = 1, Title = "Item 1", ToDoListId = 1 },
            new() { Id = 2, Title = "Item 2", ToDoListId = 1 },
            new() { Id = 3, Title = "Item 3", ToDoListId = 2 }
        };

        _mockListService.Setup(s => s.GetLists()).ReturnsAsync(lists);
        _mockItemService.Setup(s => s.GetItemsByListId(It.IsAny<int>()))
            .ReturnsAsync((int listId) => items.Where(i => i.ToDoListId == listId).ToList());

        // Act
        var result = await _controller.Index() as ViewResult;

        // Assert
        result.ShouldNotBeNull();
        result.Model.ShouldBe(lists);

        var modelLists = result.Model as List<TodoList>;
        modelLists.Count.ShouldBe(lists.Count);

        foreach (var modelList in modelLists)
        {
            var expectedItems = items.Where(i => i.ToDoListId == modelList.Id).ToList();
            modelList.Items.ShouldBe(expectedItems);
        }
    }

    [Test]
    public async Task Details_WithValidId_ReturnsViewWithTodoList()
    {
        // Arrange
        var id = 1;
        var todoList = new TodoList { Id = id, Title = "List 1" };
        _mockListService.Setup(s => s.GetListById(id)).ReturnsAsync(todoList);

        // Act
        var result = await _controller.Details(id) as ViewResult;

        // Assert
        result.ShouldNotBeNull();
        result.Model.ShouldBe(todoList);
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
    public async Task Create_ValidList_RedirectsToIndex()
    {
        // Arrange
        var todoList = new TodoList { Id = 1, Title = "List 1" };

        _mockListService.Setup(s => s.AddList(todoList)).Verifiable();

        // Act
        var result = await _controller.Create(todoList) as RedirectToActionResult;

        // Assert
        result.ShouldNotBeNull();
        result.ActionName.ShouldBe(nameof(TodoController.Index));

        _mockListService.Verify();
    }

    [Test]
    public async Task Create_InvalidList_ReturnsViewWithList()
    {
        // Arrange
        var todoList = new TodoList { Id = 1, Title = "" };
        _controller.ModelState.AddModelError("Title", "The Title field is required.");

        // Act
        var result = await _controller.Create(todoList) as ViewResult;

        // Assert
        result.ShouldNotBeNull();
        result.Model.ShouldBe(todoList);
    }


    [Test]
    public async Task Edit_Get_ValidId_ReturnsViewWithTodoList()
    {
        // Arrange
        var id = 1;
        var todoList = new TodoList { Id = id, Title = "List 1" };

        _mockListService.Setup(s => s.GetListById(id)).ReturnsAsync(todoList);

        // Act
        var result = await _controller.Edit(id) as ViewResult;

        // Assert
        result.ShouldNotBeNull();
        result.Model.ShouldBe(todoList);

        _mockListService.Verify(s => s.GetListById(id), Times.Once);
    }

    [Test]
    public async Task Edit_Get_NullId_ReturnsNotFound()
    {
        // Arrange
        int? id = null;

        // Act
        var result = await _controller.Edit(id);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();

        _mockListService.Verify(s => s.GetListById(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task Edit_Post_ValidModel_RedirectsToIndex()
    {
        // Arrange
        var id = 1;
        var todoList = new TodoList { Id = id, Title = "List 1" };

        _mockListService.Setup(s => s.UpdateList(todoList)).Verifiable();

        // Act
        var result = await _controller.Edit(id, todoList) as RedirectToActionResult;

        // Assert
        result.ShouldNotBeNull();
        result.ActionName.ShouldBe(nameof(TodoController.Index));

        _mockListService.Verify(s => s.UpdateList(todoList), Times.Once);
    }

    [Test]
    public async Task Edit_Post_InvalidModel_ReturnsViewWithTodoList()
    {
        // Arrange
        var id = 1;
        var todoList = new TodoList { Id = id, Title = "" };
        _controller.ModelState.AddModelError("Title", "The Title field is required.");

        // Act
        var result = await _controller.Edit(id, todoList) as ViewResult;

        // Assert
        result.ShouldNotBeNull();
        result.Model.ShouldBe(todoList);

        _mockListService.Verify(s => s.UpdateList(todoList), Times.Never);
    }

    [Test]
    public async Task Edit_Post_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var id = 1;
        var todoList = new TodoList { Id = id, Title = "List 1" };

        // Act
        var result = await _controller.Edit(2, todoList);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();

        _mockListService.Verify(s => s.UpdateList(todoList), Times.Never);
    }

    [Test]
    public async Task Delete_Get_ValidId_ReturnsViewWithTodoList()
    {
        // Arrange
        int id = 1;
        TodoList todoList = new TodoList { Id = id, Title = "Sample List" };
        _mockListService.Setup(s => s.GetListById(id)).ReturnsAsync(todoList);

        // Act
        var result = await _controller.Delete(id) as ViewResult;

        // Assert
        result.ShouldNotBeNull();
        result.Model.ShouldBe(todoList);
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
    public async Task Hide_ValidId_UpdatesListAndRedirectsToIndex()
    {
        // Arrange
        int id = 1;
        TodoList todoList = new TodoList { Id = id, Title = "Sample List", IsHidden = false };
        _mockListService.Setup(s => s.GetListById(id)).ReturnsAsync(todoList);

        // Act
        var result = await _controller.Hide(id) as RedirectToActionResult;

        // Assert
        result.ShouldNotBeNull();
        result.ActionName.ShouldBe("Index");

        todoList.IsHidden.ShouldBe(true);
        _mockListService.Verify(s => s.UpdateList(todoList), Times.Once);
    }

    [Test]
    public async Task Show_RedirectsToIndex()
    {
        // Arrange & Act
        var result = await _controller.Show() as RedirectToActionResult;

        // Assert
        result.ShouldNotBeNull();
        result.ActionName.ShouldBe("Index");
    }

    [Test]
    public async Task GetListById_InvalidId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var id = -1;

        // Act and Assert
        await Should.ThrowAsync<ArgumentOutOfRangeException>(async () =>
        {
            await _listService.GetListById(id);
        });
    }
    
    [Test]
    public async Task AddList_NullItem_ThrowsArgumentNullException()
    {
        // Arrange
        TodoList list = null;

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () => await _listService.AddList(list));
    }
    
    [Test]
    public async Task UpdateList_InvalidId_ThrowsInvalidOperationException()
    {
        // Arrange
        var list = new TodoList { Id = 0, Title = "Item 1" };

        // Act and Assert
        await Should.ThrowAsync<InvalidOperationException>(async () => await _listService.UpdateList(list));
    }
    
    [Test]
    public async Task UpdateRangeOfLists_NullItems_ThrowsArgumentNullException()
    {
        // Arrange
        IEnumerable<TodoList> lists = null;

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () => await _listService.UpdateRangeOfLists(lists));
    }
    
    [Test]
    public async Task DeleteList_ThrowsException()
    {
        // Arrange
        const int invalidId = -1;

        // Act & Assert
        await Should.ThrowAsync<ArgumentOutOfRangeException>(async () => await _listService.DeleteList(invalidId));
    }
    
}