using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskEntity = HomeTaste.Domain.Entities.Tasks.Tasks;

namespace HomeTaste.Application.Features.Tasks.Commands.BulkInsertTasks
{
    public class BulkInsertTasksCommandHandler : IRequestHandler<BulkInsertTasksCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;

        public BulkInsertTasksCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(BulkInsertTasksCommand request, CancellationToken cancellationToken)
        {
            var predefined = new List<(string Title, string Description, TimeSpan DueIn, TaskPriority Priority)>
            {
                ("Make Pasta", "Boil water, cook pasta, prepare sauce.", TimeSpan.FromHours(2), TaskPriority.High),
                ("Bake Cake", "Prepare batter, preheat oven, bake the cake.", TimeSpan.FromHours(4), TaskPriority.Medium),
                ("Prepare Salad", "Chop vegetables, mix salad, add dressing.", TimeSpan.FromHours(1), TaskPriority.Low),
                ("Make Pizza", "Prepare dough, add toppings, bake pizza.", TimeSpan.FromDays(1), TaskPriority.Medium),
                ("Prepare Smoothie", "Blend fruits, add milk/yogurt, serve.", TimeSpan.FromHours(0.5), TaskPriority.Low),
                ("Cook Soup", "Prepare broth, chop vegetables, cook soup.", TimeSpan.FromHours(3), TaskPriority.High),
                ("Prepare Sandwich", "Cut bread, add fillings, pack sandwich.", TimeSpan.FromHours(0.5), TaskPriority.Low),
                ("Grill Vegetables", "Season vegetables, grill until tender.", TimeSpan.FromHours(2), TaskPriority.High),
                ("Make Sandwiches", "Toast bread, add fillings like lettuce, cheese, and meats.", TimeSpan.FromHours(1), TaskPriority.Low),
                ("Cook Rice", "Boil water, add rice, cook until done.", TimeSpan.FromHours(1), TaskPriority.Medium),
                ("Prepare Stir Fry", "Chop vegetables, stir-fry with soy sauce and spices.", TimeSpan.FromHours(2), TaskPriority.Medium),
                ("Make Guacamole", "Mash avocados, add lime, onions, and tomatoes.", TimeSpan.FromHours(1), TaskPriority.Low),
                ("Prepare Pancakes", "Mix ingredients, pour batter on pan, cook pancakes.", TimeSpan.FromHours(1), TaskPriority.Low),
                ("Make Smoothie Bowl", "Blend fruits, serve in a bowl, top with granola and seeds.", TimeSpan.FromHours(0.5), TaskPriority.Low),
                ("Bake Muffins", "Mix batter, spoon into muffin tin, bake.", TimeSpan.FromHours(3), TaskPriority.Medium),
                ("Make Ice Cream", "Mix ingredients, freeze, and churn.", TimeSpan.FromDays(1), TaskPriority.Medium),
                ("Roast Chicken", "Season chicken, roast in oven until fully cooked.", TimeSpan.FromHours(5), TaskPriority.High),
                ("Make Sushi", "Prepare rice, cut fish, roll sushi.", TimeSpan.FromDays(1), TaskPriority.Medium),
                ("Prepare Soup Broth", "Boil bones, vegetables, and herbs to make broth.", TimeSpan.FromHours(2), TaskPriority.High),
                ("Make Omelette", "Whisk eggs, pour into pan, cook until set, fold in fillings.", TimeSpan.FromHours(1), TaskPriority.Low),
                ("Prepare Tacos", "Cook meat, chop veggies, assemble tacos.", TimeSpan.FromHours(2), TaskPriority.Medium),
                ("Prepare Quinoa", "Boil water, add quinoa, cook until fluffy.", TimeSpan.FromHours(1), TaskPriority.Low),
                ("Make Smoothie Popsicles", "Blend fruits, pour into molds, freeze.", TimeSpan.FromHours(4), TaskPriority.Low),
                ("Prepare Fruit Salad", "Chop fruits, mix, and serve.", TimeSpan.FromHours(1), TaskPriority.Low),
                ("Cook Mashed Potatoes", "Boil potatoes, mash, add butter and cream.", TimeSpan.FromHours(2), TaskPriority.Medium),
            };

            var tasks = predefined
                .Select(p => TaskEntity.Create(p.Title, p.Description, DateTime.Now.Add(p.DueIn), p.Priority, TasksStatus.Pending))
                .ToList();

            var newTasks = new List<TaskEntity>();

            foreach (var task in tasks)
            {
                var taskExists = await _context.Tasks.AnyAsync(t => t.Title == task.Title, cancellationToken);

                if (!taskExists)
                {
                    newTasks.Add(task);
                }
            }

            if (!newTasks.Any())
            {
                throw new ConflictException("All tasks already exist.");
            }

            _context.Tasks.AddRange(newTasks);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Ok(newTasks.Count, "New tasks related to food successfully inserted");
        }
    }
}
