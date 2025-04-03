git clone https://github.com/Azure/Commercial-Marketplace-SaaS-Accelerator.git -b 8.0.0 --depth 1; `
cd ./Commercial-Marketplace-SaaS-Accelerator/deployment; `
.\Deploy.ps1 `
 -WebAppNamePrefix "ts4-cla-offer" `
 -ResourceGroupForDeployment "ts4-cla-offer-rg" `
 -PublisherAdminUsers "nitinadmin@ezaix.com" `
 -Location "East US 2"


#  ✅ If the intallation completed without error complete the folllowing checklist:
#    🔵 Add The following URL in PartnerCenter SaaS Technical Configuration
#       ➡️ Landing Page section:       https://ts4-cla-offer-portal.azurewebsites.net/
#       ➡️ Connection Webhook section: https://ts4-cla-offer-portal.azurewebsites.net/api/AzureWebhook
#       ➡️ Tenant ID:                  1c21ad03-43bb-4990-a6dd-0b30707198c3
#       ➡️ AAD Application ID section: 25037651-4c2d-4689-93ef-7d5b8144cf38