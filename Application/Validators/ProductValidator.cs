//using FluentValidation;
//using Xenon.Application.DTOs;

//namespace Xenon.Application.Validators
//{
//    public class CreateProductValidator : AbstractValidator<CreateProductDto>
//    {
//        public CreateProductValidator()
//        {
//            RuleFor(p => p.Name)
//                .NotEmpty().WithMessage("Product name is required")
//                .Length(3, 200).WithMessage("Product name must be between 3 and 200 characters");

//            RuleFor(p => p.Description)
//                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

//            RuleFor(p => p.Price)
//                .GreaterThan(0).WithMessage("Price must be greater than 0")
//                .PrecisionScale(8, 2, true).WithMessage("Price must have up to 8 digits with 2 decimal places");

//            RuleFor(p => p.CategoryId)
//                .GreaterThan(0).WithMessage("Category must be selected");

//            RuleFor(p => p.SupplierId)
//                .GreaterThan(0).WithMessage("Supplier must be selected");
//        }
//    }

//    public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
//    {
//        public UpdateProductValidator()
//        {
//            RuleFor(p => p.ProductID)
//                .GreaterThan(0).WithMessage("Product ID is required");

//            RuleFor(p => p.Name)
//                .NotEmpty().WithMessage("Product name is required")
//                .Length(3, 200).WithMessage("Product name must be between 3 and 200 characters");

//            RuleFor(p => p.Description)
//                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

//            RuleFor(p => p.Price)
//                .GreaterThan(0).WithMessage("Price must be greater than 0")
//                .PrecisionScale(8, 2, true).WithMessage("Price must have up to 8 digits with 2 decimal places");

//            RuleFor(p => p.CategoryId)
//                .GreaterThan(0).WithMessage("Category must be selected");

//            RuleFor(p => p.SupplierId)
//                .GreaterThan(0).WithMessage("Supplier must be selected");
//        }
//    }

//    public class OrderValidator : AbstractValidator<CreateOrderDto>
//    {
//        public OrderValidator()
//        {
//            RuleFor(o => o.Name)
//                .NotEmpty().WithMessage("Name is required")
//                .MaximumLength(100);

//            RuleFor(o => o.Line1)
//                .NotEmpty().WithMessage("Address is required")
//                .MaximumLength(200);

//            RuleFor(o => o.City)
//                .NotEmpty().WithMessage("City is required")
//                .MaximumLength(100);

//            RuleFor(o => o.State)
//                .NotEmpty().WithMessage("State is required")
//                .MaximumLength(100);

//            RuleFor(o => o.Country)
//                .NotEmpty().WithMessage("Country is required")
//                .MaximumLength(100);

//            RuleFor(o => o.Lines)
//                .NotEmpty().WithMessage("Cart cannot be empty");
//        }
//    }
//}